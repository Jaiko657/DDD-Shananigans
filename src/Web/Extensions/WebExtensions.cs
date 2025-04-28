using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text.Json.Serialization;
using Web.Middleware;

namespace Web.Extensions;

public static class WebExtensions
{
    public static IServiceCollection AddWeb(this IServiceCollection services)
    {
        services.AddControllers()
            .AddJsonOptions(opt =>
            {
                opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });;
        return services.AddApiServices();
    }
    
    private static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        // Add API Explorer for Swagger
        services.AddEndpointsApiExplorer();
        
        var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0";
        var projectName = Assembly.GetExecutingAssembly().GetName().Name ?? "Web API";
        
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo 
            { 
                Title = projectName, 
                Version = "v1",
                Description = $"Build Version: {version}"
            });
        });
        
        return services;
    }
    
    public static IApplicationBuilder UseWebApiFeatures(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (!env.IsDevelopment()) return app;
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0";
            c.SwaggerEndpoint("/swagger/v1/swagger.json", $"API V1 (Build {version})");
            c.RoutePrefix = string.Empty;
        });

        return app;
    }
}