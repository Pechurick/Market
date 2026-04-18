using FluentValidation;
using Market.Application.Models.Requests;

namespace Market.Application.Validators;

public class OrderCreateDtoValidator : AbstractValidator<OrderCreateDto>
{
    public OrderCreateDtoValidator()
    {
        // 1. Перевіряємо, щоб список не був порожнім (не можна оформити пусте замовлення)
        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Замовлення не може бути порожнім. Додайте хоча б один товар у кошик.");

        // 2. МАГІЯ: Застосовуємо попередній валідатор до КОЖНОГО елемента в списку!
        RuleForEach(x => x.Items).SetValidator(new OrderItemCreateDtoValidator());
    }
}
