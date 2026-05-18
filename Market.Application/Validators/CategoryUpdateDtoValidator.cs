using FluentValidation;
using Market.Application.Models.Requests;

namespace Market.Application.Validators;

public class CategoryUpdateDtoValidator : AbstractValidator<CategoryUpdateDto>
{
    public CategoryUpdateDtoValidator()
    {
        
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Назва категорії не може бути порожньою або складатися лише з пробілів.")
            .MinimumLength(2).WithMessage("Назва категорії має містити мінімум 2 символи.")
            .MaximumLength(100).WithMessage("Назва категорії не може перевищувати 100 символів.");
    }
}
