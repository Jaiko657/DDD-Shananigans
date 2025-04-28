namespace Core.Features.Orders.DTOs;

public sealed record OrderDto(Guid Id, string Customer, decimal TotalAmount, DateTime CreatedOn);