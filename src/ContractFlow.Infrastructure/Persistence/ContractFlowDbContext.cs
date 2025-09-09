using ContractFlow.Domain.Models;
using ContractFlow.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;

namespace ContractFlow.Infrastructure.Persistence;

public sealed class ContractFlowDbContext(DbContextOptions<ContractFlowDbContext> options) : DbContext(options) 
{
    public DbSet<Contract> Contracts => Set<Contract>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>(); 

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ContractFlowDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
