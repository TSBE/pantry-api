using System;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.V1.Controllers.Requests;

namespace Pantry.Features.WebFeature.V1.Controllers.Extensions;

public static class ArticleRequestExtensions
{
    public static Article ToModelNotNull(this ArticleRequest dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        return new Article
        {
            StorageLocationId = dto.StorageLocationId,
            GlobalTradeItemNumber = dto.GlobalTradeItemNumber,
            Name = dto.Name,
            BestBeforeDate = dto.BestBeforeDate,
            Quantity = dto.Quantity,
            Content = dto.Content,
            ContentType = dto.ContentType.ToModelNotNull()
        };
    }
}
