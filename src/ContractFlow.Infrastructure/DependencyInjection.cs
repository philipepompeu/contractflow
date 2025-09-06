using ContractFlow.Application.Contracts.Abstractions;
using ContractFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;



namespace ContractFlow.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructurePersistence(this IServiceCollection services, IConfiguration configuration)
        {

            var cs = configuration.GetConnectionString("postgres") ?? throw new InvalidOperationException("ConnectionStrings:postgres não configurada.");

            services.AddDbContext<ContractFlowDbContext>(options =>
            {
                options.UseNpgsql(cs, npg =>
                {
                    npg.MigrationsAssembly(typeof(ContractFlowDbContext).Assembly.FullName);
                    //npg.MigrationsHistoryTable("migrations_history");
                });
            });

            services.AddScoped<IContractWriteRepository, Repositories.ContractWriteRepository>();

            return services;
        }
    }
}