using FluentValidation;

namespace Pantry.Features.WebFeature.V1.Controllers.Requests;

public class StorageLocationRequestValidator : AbstractValidator<StorageLocationRequest>
{
    public StorageLocationRequestValidator()
    {
        RuleFor(x => x.Name).NotNull().NotEmpty();
        RuleFor(x => x.Description);
    }
}
