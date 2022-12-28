using FluentValidation;

namespace Pantry.Features.WebFeature.V1.Controllers.Requests;

public class HouseholdRequestValidator : AbstractValidator<HouseholdRequest>
{
    public HouseholdRequestValidator()
    {
        RuleFor(x => x.Name).NotNull().NotEmpty();
        RuleFor(x => x.SubscriptionType).NotNull().NotEmpty().IsInEnum();
    }
}
