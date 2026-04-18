using FluentValidation;
using Market.Application.Models.Requests;

namespace Market.Application.Validators;

// Правила зазвичай такі ж самі, як і при створенні
public class UserUpdateDtoValidator : AbstractValidator<UserUpdateDto>
{
    public UserUpdateDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Ім'я не може бути порожнім або складатися лише з пробілів.")
            .MaximumLength(50).WithMessage("Ім'я не може перевищувати 50 символів.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Прізвище не може бути порожнім.")
            .MaximumLength(50).WithMessage("Прізвище не може перевищувати 50 символів.");

        RuleFor(x => x.MiddleName)
            .MaximumLength(50).WithMessage("По батькові не може перевищувати 50 символів.");
    }
}
