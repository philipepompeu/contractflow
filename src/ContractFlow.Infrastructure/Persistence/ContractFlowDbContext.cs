using ContractFlow.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace ContractFlow.Infrastructure.Persistence;

public sealed class ContractFlowDbContext(DbContextOptions<ContractFlowDbContext> options) : DbContext(options) 
{
    public DbSet<Contract> Contracts => Set<Contract>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ContractFlowDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
