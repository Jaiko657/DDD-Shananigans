using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Core.Behaviors;
using MediatR;
using System.Reflection;

namespace Core.Extensions;

public static class CoreExtensions
{
    public static IServiceCollection AddCore(this IServiceCollection services)
        => services
            .AddMediatRServices()
            .AddValidationServices()
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

    private static IServiceCollection AddMediatRServices(this IServiceCollection services)
        => services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CoreExtensions).Assembly));

    private static IServiceCollection AddValidationServices(this IServiceCollection services)
        => services.AddValidatorsFromAssembly(typeof(CoreExtensions).Assembly);
}