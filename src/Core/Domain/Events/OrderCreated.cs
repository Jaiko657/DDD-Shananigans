using Core.Domain.Common;
using MediatR;

namespace Core.Domain.Events;

public sealed record OrderCreated(Guid OrderId) : BaseEvent;