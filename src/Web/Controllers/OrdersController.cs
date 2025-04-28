using Core.Features.Orders.Commands;
using Core.Features.Orders.DTOs;
using Core.Features.Orders.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new order
    /// </summary>
    /// <remarks>
    /// Valid Sample request:
    ///
    ///     POST /api/orders
    ///     {
    ///        "customer": "John Doe",
    ///        "totalAmount": 100.50
    ///     }
    /// Exceptions:
    /// Will trigger ValidationException:
    ///     - Empty customer name
    ///
    /// Will trigger DomainException:
    ///     - TotalAmount <= 0
    ///     - TotalAmount > 10000
    /// </remarks>
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateOrder([FromBody] CreateOrderCommand command)
    {
        var orderId = await _mediator.Send(command);
        return CreatedAtAction(nameof(Get), new { id = orderId }, orderId);
    }

    [HttpGet("{id:guid}")]
    public Task<OrderDto?> Get(Guid id) => _mediator.Send(new GetOrderQuery(id));
}