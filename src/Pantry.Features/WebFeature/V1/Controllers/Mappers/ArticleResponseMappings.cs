using System;
using System.Collections.Generic;
using Pantry.Core.Mappers;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.V1.Controllers.Responses;

namespace Pantry.Features.WebFeature.V1.Controllers.Mappers
{
    public static class ArticleResponseMappings
    {
        private static readonly DtoModelMapping<ArticleResponse, Article> Mapping =
            new(
                null,
                config =>
                {
                    config.ForMember(o => o.Id, dest => dest.MapFrom(o => o.ArticleId));
                    config.ForMember(o => o.StorageLocationId, dest => dest.MapFrom(o => o.StorageLocationId));
                    config.ForMember(o => o.GlobalTradeItemNumber, dest => dest.MapFrom(o => o.GlobalTradeItemNumber));
                    config.ForMember(o => o.Name, dest => dest.MapFrom(o => o.Name));
                    config.ForMember(o => o.BestBeforeDate, dest => dest.MapFrom(o => o.BestBeforeDate));
                    config.ForMember(o => o.Quantity, dest => dest.MapFrom(o => o.Quantity));
                    config.ForMember(o => o.Content, dest => dest.MapFrom(o => o.Content));
                    config.ForMember(o => o.ContentType, dest => dest.MapFrom(o => o.ContentType));
                });

        public static ArticleResponse? ToDto(this Article model)
        {
            return Mapping.ToDto(model);
        }

        public static ArticleResponse ToDtoNotNull(this Article model)
        {
            return Mapping.ToDtoNotNull(model);
        }

        public static ArticleResponse[] ToDtos(this IEnumerable<Article> models)
        {
            return Mapping.ToDtos(models);
        }
    }
}
