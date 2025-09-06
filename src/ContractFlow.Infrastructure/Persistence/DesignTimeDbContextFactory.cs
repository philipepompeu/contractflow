using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;

namespace ContractFlow.Infrastructure.Persistence;

public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ContractFlowDbContext>
{
    public ContractFlowDbContext CreateDbContext(string[] args)
    {
        // Lê appsettings.Development.json da API
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "ContractFlow.Api");
        var cfg = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables().Build();

        var cs = cfg.GetConnectionString("postgres") ?? throw new InvalidOperationException("ConnectionStrings:postgres não configurada.");

        var options = new DbContextOptionsBuilder<ContractFlowDbContext>()
            .UseNpgsql(cs)
            .Options;

        return new ContractFlowDbContext(options);
    }
}
