using Core.Abstractions;
using Core.CQRS.Command;
using Core.Domain.Entities;

namespace Core.Features.Orders.Commands;

public sealed class CreateOrderHandler : ICommandHandler<CreateOrderCommand>
{
    private readonly IRepository<Order> _repo;

    public CreateOrderHandler(IRepository<Order> repo) => _repo = repo;

    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken ct)
    {
        var order = Order.Create(request.Customer, request.TotalAmount, request.Type);
        await _repo.AddAsync(order, ct);
        return order.Id;
    }
}