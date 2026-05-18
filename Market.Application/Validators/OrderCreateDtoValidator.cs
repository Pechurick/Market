using FluentValidation;
using Market.Application.Models.Requests;

namespace Market.Application.Validators;

public class OrderCreateDtoValidator : AbstractValidator<OrderCreateDto>
{
    public OrderCreateDtoValidator()
    {
        
        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Замовлення не може бути порожнім. Додайте хоча б один товар у кошик.");

        
        RuleForEach(x => x.Items).SetValidator(new OrderItemCreateDtoValidator());
    }
}
