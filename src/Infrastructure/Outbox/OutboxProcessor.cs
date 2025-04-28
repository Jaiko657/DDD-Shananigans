
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Outbox;

public sealed class OutboxProcessor : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxProcessor> _logger;

    public OutboxProcessor(IServiceScopeFactory scopeFactory, ILogger<OutboxProcessor> logger)
    {
        _scopeFactory = scopeFactory;
        _logger       = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            using var scope   = _scopeFactory.CreateScope();
            var db            = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var mediator      = scope.ServiceProvider.GetRequiredService<IMediator>();

            var batch = await db.OutboxMessages
                .Where(x => x.ProcessedOnUtc == null)
                .OrderBy(x => x.OccurredOnUtc)
                .Take(50)
                .ToListAsync(token);

            foreach (var msg in batch)
            {
                try
                {
                    var type  = Type.GetType(msg.Type)!;
                    var @event = JsonSerializer.Deserialize(msg.Content, type)!;
                    await mediator.Publish(@event, token);

                    msg.ProcessedOnUtc = DateTime.UtcNow;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed processing outbox {Id}", msg.Id);
                    msg.Error = ex.ToString();
                }
            }

            await db.SaveChangesAsync(token);
            await Task.Delay(TimeSpan.FromSeconds(5), token);
        }
    }
}
