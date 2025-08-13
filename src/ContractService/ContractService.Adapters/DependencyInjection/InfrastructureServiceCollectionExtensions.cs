using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ContractService.Ports.Outbound;
using ContractService.Infrastructure.Data;
using ContractService.Adapters.Outbound.Repositories;
using ContractService.Adapters.Outbound.Services;
using ContractService.Adapters.Outbound.Messaging;
using InsuranceSystem.Shared.Infrastructure.Configuration;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using MassTransit;
using ContractService.Infrastructure.Messaging;
using InsuranceSystem.Shared.Infrastructure.Messaging;

namespace ContractService.Adapters.DependencyInjection;

[ExcludeFromCodeCoverage]
public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Adiciona serviços compartilhados
        services.AddSharedServices();

        services.AddDbContext<ContractDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ContractDbContext).Assembly.FullName)
            ));

        // Redis Cache
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration["Redis:ConnectionString"]; // ex: localhost:6379
            options.InstanceName = configuration["Redis:InstanceName"] ?? "contracts:";
        });

        // Repositorio base + decorator em cache
        services.AddScoped<ContractRepository>();
        services.AddScoped<IContractRepositoryPort, CachedContractRepository>();

        services.AddScoped<IProposalServicePort, ProposalServiceClient>();
        services.AddScoped<IContractNumberGeneratorPort, ContractNumberGenerator>();

        // MassTransit / RabbitMQ - Publisher + Consumer
        services.AddMassTransit(x =>
        {
            // Registrar consumidores
            x.AddConsumer<ProposalStatusUpdatedConsumer>();
            
            x.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(configuration["RabbitMq:Host"], h =>
                {
                    var user = configuration["RabbitMq:Username"];
                    var pass = configuration["RabbitMq:Password"];
                    if (!string.IsNullOrWhiteSpace(user)) h.Username(user);
                    if (!string.IsNullOrWhiteSpace(pass)) h.Password(pass);
                });
                
                // Configurar endpoints dos consumidores
                cfg.ConfigureEndpoints(ctx);
            });
        });
        services.AddScoped<IEventPublisherPort, ContractService.Adapters.Outbound.Messaging.MassTransitEventPublisher>();

        return services;
    }
} 