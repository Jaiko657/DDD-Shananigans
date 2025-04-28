using Core.CQRS.Query;
using Core.Features.Orders.DTOs;

namespace Core.Features.Orders.Queries;

public sealed record GetOrderQuery(Guid Id) : IQuery<OrderDto?>;