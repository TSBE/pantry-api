using FluentValidation;

namespace Pantry.Features.WebFeature.V1.Controllers.Requests;

public class ArticleRequestValidator : AbstractValidator<ArticleRequest>
{
    public ArticleRequestValidator()
    {
        RuleFor(x => x.StorageLocationId).NotNull().NotEmpty().GreaterThan(0);
        RuleFor(x => x.GlobalTradeItemNumber);
        RuleFor(x => x.Name).NotNull().NotEmpty();
        RuleFor(x => x.BestBeforeDate).NotNull().NotEmpty();
        RuleFor(x => x.Quantity).NotNull().GreaterThanOrEqualTo(0);
        RuleFor(x => x.Content);
        RuleFor(x => x.ContentType).NotNull().IsInEnum();
    }
}
