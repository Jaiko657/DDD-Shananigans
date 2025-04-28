using MediatR;

namespace Core.Domain.Common;

public abstract record BaseEvent : INotification;
