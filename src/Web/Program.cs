using Core.Extensions;
using Infrastructure.Extensions;
using Serilog;
using Web.Extensions;
using Web.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration));

builder.Services
    .AddCore()
    .AddInfrastructure(builder.Configuration)
    .AddWeb();

var app = builder.Build();

app.UseWebApiFeatures(app.Environment);

// Use the exception handling middleware
app.UseExceptionHandlingMiddleware();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();