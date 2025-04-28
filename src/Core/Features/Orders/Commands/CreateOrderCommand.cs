using Core.CQRS.Command;
using Core.Domain.Enums;

namespace Core.Features.Orders.Commands;

public sealed record CreateOrderCommand(string Customer, decimal TotalAmount, string Type) : ICommand;