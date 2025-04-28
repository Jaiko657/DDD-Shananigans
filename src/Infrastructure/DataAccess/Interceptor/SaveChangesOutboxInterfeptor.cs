using Core.Domain.Primatives;
using Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.DataAccess.Interceptor;

public sealed class SaveChangesOutboxInterceptor : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData data,
        InterceptionResult<int> result,
        CancellationToken token = default)
    {
        var context = data.Context!;
        var messages =
            context.ChangeTracker.Entries<AggregateRoot>()
                .SelectMany(e => e.Entity.DomainEvents)
                .Select(OutboxMessage.FromDomainEvent)
                .ToList();

        if (messages.Count == 0) return await base.SavingChangesAsync(data, result, token);
        context.Set<OutboxMessage>().AddRange(messages);

        foreach (var entry in context.ChangeTracker.Entries<AggregateRoot>())
            entry.Entity.ClearDomainEvents();

        return await base.SavingChangesAsync(data, result, token);
    }
}