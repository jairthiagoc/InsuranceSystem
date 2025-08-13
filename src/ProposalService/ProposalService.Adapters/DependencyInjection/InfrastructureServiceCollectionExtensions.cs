using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProposalService.Ports.Outbound;
using ProposalService.Infrastructure.Data;
using ProposalService.Adapters.Outbound.Repositories;
using ProposalService.Adapters.Outbound.Messaging;
using InsuranceSystem.Shared.Infrastructure.Configuration;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using MassTransit;

namespace ProposalService.Adapters.DependencyInjection;

[ExcludeFromCodeCoverage]
public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Adiciona serviços compartilhados
        services.AddSharedServices();

        services.AddDbContext<ProposalDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ProposalDbContext).Assembly.FullName)
                    .EnableRetryOnFailure()
            ));

        // Redis Cache
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration["Redis:ConnectionString"]; // ex: localhost:6379
            options.InstanceName = configuration["Redis:InstanceName"] ?? "proposals:";
        });

        // Repositorio base + decorator
        services.AddScoped<ProposalRepository>();
        // Temporariamente desabilitando cache para testes
        // services.AddScoped<IProposalRepositoryPort, CachedProposalRepository>();
        services.AddScoped<IProposalRepositoryPort, ProposalRepository>();

        // MassTransit / RabbitMQ - Publisher
        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(configuration["RabbitMq:Host"], h =>
                {
                    var user = configuration["RabbitMq:Username"];
                    var pass = configuration["RabbitMq:Password"];
                    if (!string.IsNullOrWhiteSpace(user)) h.Username(user);
                    if (!string.IsNullOrWhiteSpace(pass)) h.Password(pass);
                });
            });
        });
        services.AddScoped<IEventPublisherPort, MassTransitEventPublisher>();

        return services;
    }
} 