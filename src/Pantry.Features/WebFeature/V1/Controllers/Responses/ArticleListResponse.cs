using System.Collections.Generic;

namespace Pantry.Features.WebFeature.V1.Controllers.Responses;

public class ArticleListResponse
{
    public IEnumerable<ArticleResponse>? Articles { get; set; }
}
