using FluentValidation;
using Market.Application.Models.Requests;

namespace Market.Application.Validators;

public class BrandUpdateDtoValidator : AbstractValidator<BrandUpdateDto>
{
    public BrandUpdateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Назва бренду не може бути порожньою.")
            .MinimumLength(2).WithMessage("Назва бренду має містити мінімум 2 символи.")
            .MaximumLength(100).WithMessage("Назва бренду не може перевищувати 100 символів.");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Опис бренду занадто довгий.");
    }
}
