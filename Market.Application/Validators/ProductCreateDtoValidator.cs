using FluentValidation;
using Market.Application.Models.Requests;

namespace Market.Application.Validators;

public class ProductCreateDtoValidator : AbstractValidator<ProductCreateDto>
{
    public ProductCreateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Назва товару не може бути порожньою.")
            .MinimumLength(2).WithMessage("Назва товару має містити мінімум 2 символи.")
            .MaximumLength(200).WithMessage("Назва товару не може перевищувати 200 символів.");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Товар обов'язково повинен належати до певної категорії (CategoryId не може бути порожнім).");

        RuleFor(x => x.Price)
            .NotNull().WithMessage("Ціна товару є обов'язковою.") 
            .GreaterThan(0).WithMessage("Ціна товару має бути більшою за нуль.");

        RuleFor(x => x.Amount) 
            .NotNull().WithMessage("Кількість товару є обов'язковою.") 
            .GreaterThanOrEqualTo(0).WithMessage("Кількість товару не може бути від'ємною.");
    }
}
