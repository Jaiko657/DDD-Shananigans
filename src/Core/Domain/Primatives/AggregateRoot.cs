using System.ComponentModel.DataAnnotations.Schema;
using Core.Domain.Common;

namespace Core.Domain.Primatives;

public abstract class AggregateRoot : Entity
{
    private readonly List<object> _domainEvents = new();
    
    protected AggregateRoot(Guid id) : base(id) { }

    [NotMapped]
    public IReadOnlyCollection<object> DomainEvents => _domainEvents.AsReadOnly();

    protected void RaiseDomainEvent(BaseEvent? eventItem)
    {
        if (eventItem == null) return;
        _domainEvents.Add(eventItem);
    }

    public void ClearDomainEvents() => _domainEvents.Clear();
}