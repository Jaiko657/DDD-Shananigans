using Core.Abstractions;
using FluentValidation;
using Infrastructure.DataAccess;
using Infrastructure.DataAccess.Interceptor;
using Infrastructure.Outbox;
using Infrastructure.Pipelines;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration cfg)
    {
        var connectionString = cfg.GetConnectionString("Default");

        services.AddSingleton<SaveChangesOutboxInterceptor>();

        services.AddDbContext<AppDbContext>((sp, opts) =>
        {
            opts.UseSqlServer(connectionString);
            opts.AddInterceptors(sp.GetRequiredService<SaveChangesOutboxInterceptor>());
        });

        services
            .AddScoped(typeof(IRepository<>), typeof(EfRepository<>))
            .AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>())
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>))
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>))
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(UnitOfWorkBehavior<,>))
            .AddValidatorsFromAssembly(typeof(AppDbContext).Assembly)
            .AddMediatR(m => m.RegisterServicesFromAssembly(typeof(AppDbContext).Assembly))
            .AddHostedService<OutboxProcessor>();

        return services;
    }
}