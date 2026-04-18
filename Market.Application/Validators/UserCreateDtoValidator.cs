using FluentValidation;
using Market.Application.Models.Requests;

namespace Market.Application.Validators;

public class UserCreateDtoValidator : AbstractValidator<UserCreateDto>
{
    public UserCreateDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Ім'я не може бути порожнім або складатися лише з пробілів.")
            .MaximumLength(50).WithMessage("Ім'я не може перевищувати 50 символів.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Прізвище не може бути порожнім.")
            .MaximumLength(50).WithMessage("Прізвище не може перевищувати 50 символів.");

        // MiddleName зазвичай необов'язкове, тому перевіряємо лише довжину, якщо воно є
        RuleFor(x => x.MiddleName)
            .MaximumLength(50).WithMessage("По батькові не може перевищувати 50 символів.");
            
        // Якщо у тебе в DTO є Email, обов'язково додай це:
        // RuleFor(x => x.Email)
        //     .NotEmpty().WithMessage("Email є обов'язковим.")
        //     .EmailAddress().WithMessage("Невірний формат Email.");
    }
}
