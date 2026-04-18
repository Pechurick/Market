using FluentValidation;
using Market.Application.Models.Requests;

namespace Market.Application.Validators;

public class PagedRequestDtoValidator : AbstractValidator<PagedRequestDto>
{
    public PagedRequestDtoValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Номер сторінки має бути рівним 1 або більшим.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Розмір сторінки має бути в межах від 1 до 100 елементів.");
    }
}