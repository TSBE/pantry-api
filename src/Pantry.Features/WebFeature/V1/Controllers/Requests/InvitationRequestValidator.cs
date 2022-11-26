using FluentValidation;

namespace Pantry.Features.WebFeature.V1.Controllers.Requests;

public class InvitationRequestValidator : AbstractValidator<InvitationRequest>
{
    public InvitationRequestValidator()
    {
        RuleFor(x => x.FriendsCode).NotNull().NotEmpty();
    }
}
