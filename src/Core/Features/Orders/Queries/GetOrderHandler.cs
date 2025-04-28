using Core.Abstractions;
using Core.CQRS.Query;
using Core.Domain.Entities;
using Core.Features.Orders.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Core.Features.Orders.Queries;

public sealed class GetOrderHandler : IQueryHandler<GetOrderQuery, OrderDto?>
{
    private readonly IRepository<Order> _repo;

    public GetOrderHandler(IRepository<Order> repo) => _repo = repo;

    public async Task<OrderDto?> Handle(GetOrderQuery request, CancellationToken ct)
    {
        return await _repo.Query()
            .Where(o => o.Id == request.Id)
            .Select(o => new OrderDto(o.Id, o.Customer, o.TotalAmount, o.CreatedOn))
            .SingleOrDefaultAsync(ct);
    }
}