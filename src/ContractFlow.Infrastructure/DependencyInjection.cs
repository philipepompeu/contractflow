using ContractFlow.Application.Contracts.Abstractions;
using ContractFlow.Infrastructure.Outbox;
using ContractFlow.Infrastructure.Persistence;
using MassTransit;
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

            services.AddScoped<OutboxSaveChangesInterceptor>();            

            services.AddDbContext<ContractFlowDbContext>((serviceProvider, options) =>
            {
                options.UseNpgsql(cs, npg =>
                {
                    npg.MigrationsAssembly(typeof(ContractFlowDbContext).Assembly.FullName);
                    //npg.MigrationsHistoryTable("migrations_history");
                });

                // injeta o interceptor
                options.AddInterceptors(serviceProvider.GetRequiredService<OutboxSaveChangesInterceptor>());
            });

            services.AddScoped<IContractWriteRepository, Repositories.ContractWriteRepository>();

            return services;
        }
        
        public static IServiceCollection AddInfrastructureMessaging(this IServiceCollection services, IConfiguration cfg)
        {
            services.AddMassTransit(x =>
            {
                // (consumers entram depois; por enquanto só Publish)
                x.UsingRabbitMq((context, bus) =>
                {
                    
                    bus.Host(cfg["Rabbit:Host"] ?? "localhost", 5673,"/", h =>
                    {
                        h.Username(cfg["Rabbit:User"] ?? "guest");
                        h.Password(cfg["Rabbit:Pass"] ?? "guest");                        
                    });
                });
            });

            return services;
        }

        public static IServiceCollection AddOutboxDispatcher(this IServiceCollection services, IConfiguration cfg)
        {
            services.Configure<OutboxOptions>(cfg.GetSection("Outbox"));
            services.AddHostedService<OutboxDispatcher>();
            return services;
        }
    }
}