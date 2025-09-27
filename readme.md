# ContractFlow

A small, study-focused .NET 8 project that demonstrates **CQRS with MediatR**, a **rich domain** modeled with **EF Core (Fluent API)**, **MassTransit + RabbitMQ** for messaging, a **Transactional Outbox**, **Serilog** for structured logging, **FluentValidation** for input validation, and **basic correlation/telemetry** propagation end‑to‑end.

---
## Architecture

**Solution layout (classic clean-ish layering):**

- **ContractFlow.Domain** — Rich domain (aggregates, value objects, domain events).
- **ContractFlow.Application** — CQRS (Commands/Queries/Handlers), validators, pipeline behaviors, ports (interfaces).
- **ContractFlow.Infrastructure** — EF Core (DbContext + Fluent mappings), Outbox, MassTransit (bus, consumers), repositories.
- **ContractFlow.Api** — Minimal API endpoints, DI composition, Serilog bootstrap.

**Data store:** PostgreSQL  
**Broker:** RabbitMQ (AMQP 0-9-1) via MassTransit

---

## Key Concepts Covered

### CQRS + MediatR
- Commands and Queries are modeled as records. Handlers live in **Application**.
- A **ValidationBehavior** pipeline runs **FluentValidation** before the handlers.

### Rich Domain (DDD‑lite)
- `Contract` is an **abstract aggregate root** with two concrete types (TPH inheritance):
    - `SaleContract`
    - `PurchaseContract`
- Factory methods are centralized under `Contract.Factories` to protect invariants.
- Domain methods like `Activate()` keep business rules inside the aggregate.

### EF Core (Fluent API) with Postgres
- **TPH (Table‑per‑Hierarchy)** inheritance for contracts; a `contract_type` discriminator in a single `contracts` table.
- **Owned Entity Types**: `Money` value object mapped into columns (`total_amount`, `total_currency`).
- No data annotations in the domain — mapping lives in Infrastructure.

### Transactional Outbox
- Domain events implement a small `IDomainEvent` abstraction.
- A `SaveChangesInterceptor` collects **domain events** and persists them to `outbox_messages` **in the same transaction**.
- A hosted service (**OutboxDispatcher**) drains the outbox, rehydrates the event, and **publishes** to RabbitMQ via MassTransit with retry/backoff.

### Messaging (MassTransit + RabbitMQ)
- Outbox publishes events of domain types (e.g., `ContractCreatedDomainEvent`, `ContractApprovedDomainEvent`).
- Consumers in Infrastructure react to those events to create or update read models or trigger side effects.

### Validation
- **FluentValidation** for input/shape validation at the Application boundary.
- Invariants/business rules live in the domain model.

### Logging
- **Serilog** as the logging provider (JSON output, request logging).

---

## Domain & Flow Overview

The project models **contracts** and a simple **approval** workflow.

**Happy‑path flow:**

```
Client ── POST /contracts
  → Application handler creates domain aggregate (Sale or Purchase)
    → EF saves Contract
    → Domain event ContractCreatedDomainEvent captured by Outbox (same TX)
    → OutboxDispatcher publishes event to RabbitMQ
      → Consumer creates an ApprovalDocument (Pending)

Client ── GET /approvals/pending
  ← List of pending approval documents

Client ── POST /approvals/{id}/approve
  → Application handler marks ApprovalDocument as Approved
    → Domain event ContractApprovedDomainEvent captured by Outbox
    → OutboxDispatcher publishes event to RabbitMQ
      → Consumer loads Contract and calls contract.Activate()
```

## Run It Locally

### Prerequisites
- **.NET SDK 8**
- **Docker** and **docker compose**

### 1) Start dependencies
From the repo root:
```bash
docker compose up -d
# This brings up Postgres and RabbitMQ (management UI at http://localhost:15672).
```

### 2) Apply database migrations
```bash
dotnet tool update --global dotnet-ef
dotnet ef database update   --project src/ContractFlow.Infrastructure   --startup-project src/ContractFlow.Api
```

### 3) Run the API
```bash
dotnet run --project src/ContractFlow.Api
```

### 4) Try it out
```bash
# Create a Sale contract (type=2) starting tomorrow (adjust the date)
curl -s -X POST http://localhost:5000/contracts   -H "Content-Type: application/json"   -d '{"type":2,"partyId":"11111111-1111-1111-1111-111111111111","planId":"22222222-2222-2222-222222222222","startDate":"2025-10-01","amount":129.90,"currency":"BRL"}' | jq

# List pending approvals
curl -s "http://localhost:5000/approvals/pending?page=1&size=20" | jq

# Approve one
curl -s -X POST http://localhost:5000/approvals/<approvalId>/approve
```

> The outbox dispatcher will publish events to Rabbit; consumers process them to create approval documents (on create) and to activate the contract (on approve).