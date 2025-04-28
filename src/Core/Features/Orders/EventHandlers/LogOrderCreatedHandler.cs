using Core.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Features.Orders.EventHandlers;

public class LogOrderCreatedHandler : INotificationHandler<OrderCreated>
{
    private readonly ILogger<LogOrderCreatedHandler> _logger;

    public LogOrderCreatedHandler(ILogger<LogOrderCreatedHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(OrderCreated notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Domain Event Handled: Order with ID {OrderId} was successfully created.",
            notification.OrderId);

        return Task.CompletedTask;
    }
}