using Core.Domain.Enums;
using FluentValidation;

namespace Core.Features.Orders.Commands;

public class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.Customer)
            .NotEmpty().WithMessage("Customer name required")
            .MaximumLength(100);
            
        RuleFor(x => x.TotalAmount)
            .GreaterThan(0).WithMessage("Total amount must be greater than zero")
            .LessThanOrEqualTo(10000).WithMessage("Total amount cannot exceed 10000");

        RuleFor(x => x.Type)
            .Must(v => Enum.TryParse<OrderType>(v, true, out _))
            .WithMessage($"Order type must be one of the following: {string.Join(", ", Enum.GetNames(typeof(OrderType)))}");
    }
}