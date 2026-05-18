using FluentValidation;
using Market.Application.Models.Requests;

namespace Market.Application.Validators;

public class OrderItemCreateDtoValidator : AbstractValidator<OrderItemCreateDto> 
{
    public OrderItemCreateDtoValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Потрібно вказати ID товару.");

        RuleFor(x => x.Amount)
            .NotNull().WithMessage("Кількість товару є обов'язковою.")
            .GreaterThan(0).WithMessage("Ви не можете замовити 0 або менше товарів. Мінімум 1.");
    }
}
