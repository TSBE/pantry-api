using FluentValidation;

namespace Pantry.Features.WebFeature.V1.Controllers.Requests;

public class DeviceUpdateRequestValidator : AbstractValidator<DeviceUpdateRequest>
{
    public DeviceUpdateRequestValidator()
    {
        RuleFor(x => x.Name).NotNull().NotEmpty();
        RuleFor(x => x.DeviceToken);
    }
}
