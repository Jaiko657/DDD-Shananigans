using Core.Abstractions.Exceptions;
using Core.Domain.Common;
using Core.Domain.Enums;
using Core.Domain.Events;
using Core.Domain.Primatives;

namespace Core.Domain.Entities;

public class Order : AggregateRoot
{
    public string Customer { get; private set; }
    public OrderType Type { get; private set; }
    public decimal TotalAmount { get; private set; }
    public DateTime CreatedOn { get; private set; } = DateTime.UtcNow;

    private Order(Guid id, string customer, decimal totalAmount, OrderType type) : base(id)
    {
        Customer = customer;
        TotalAmount = totalAmount;
        Type = type;
    }

    public static Order Create(string customer, decimal totalAmount, string type)
    {
        if (!Enum.TryParse<OrderType>(type, true, out var orderType))
            throw new DomainException("Invalid order type", "Order.CreationException");
        Order order = new(Guid.NewGuid(), customer, totalAmount, orderType);
        Validate(order);
        order.RaiseDomainEvent(new OrderCreated(order.Id));
        return order;
    }

    private static void Validate(Order order)
    {
        switch (order.Type)
        {
            case OrderType.International:
                if (order.TotalAmount > 500) throw new DomainException("International Orders Cannot Exceed 500", "Order.CreationException");
                break;
            case OrderType.Internal:
            default:
                break;
        }
    }
}