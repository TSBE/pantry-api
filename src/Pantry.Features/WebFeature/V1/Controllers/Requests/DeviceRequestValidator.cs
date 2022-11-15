using FluentValidation;

namespace Pantry.Features.WebFeature.V1.Controllers.Requests;

public class DeviceRequestValidator : AbstractValidator<DeviceRequest>
{
    public DeviceRequestValidator()
    {
        RuleFor(x => x.InstallationId).NotNull().NotEmpty();
        RuleFor(x => x.Model).NotNull().NotEmpty();
        RuleFor(x => x.Name).NotNull().NotEmpty();
        RuleFor(x => x.Platform).NotNull().NotEmpty().IsInEnum();
    }
}
