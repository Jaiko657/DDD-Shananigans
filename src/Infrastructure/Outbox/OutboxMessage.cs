using System.Text.Json;

namespace Infrastructure.Outbox;

public sealed class OutboxMessage
{
    public Guid      Id              { get; init; } = Guid.NewGuid();
    public DateTime  OccurredOnUtc   { get; init; }
    public string    Type            { get; init; } = null!;
    public string    Content         { get; init; } = null!;
    public DateTime? ProcessedOnUtc  { get; set; }
    public string?   Error           { get; set; }

    public static OutboxMessage FromDomainEvent(object @event) =>
        new()
        {
            OccurredOnUtc = DateTime.UtcNow,
            Type          = @event.GetType().AssemblyQualifiedName!,
            Content       = JsonSerializer.Serialize(@event)
        };
}