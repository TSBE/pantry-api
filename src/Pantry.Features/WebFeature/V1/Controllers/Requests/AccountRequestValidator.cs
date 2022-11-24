using FluentValidation;

namespace Pantry.Features.WebFeature.V1.Controllers.Requests;

public class AccountRequestValidator : AbstractValidator<AccountRequest>
{
    public AccountRequestValidator()
    {
        RuleFor(x => x.FirstName).NotNull().NotEmpty();
        RuleFor(x => x.LastName).NotNull().NotEmpty();
    }
}
