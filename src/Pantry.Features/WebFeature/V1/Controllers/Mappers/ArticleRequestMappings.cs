using Pantry.Core.Mappers;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.V1.Controllers.Requests;

namespace Pantry.Features.WebFeature.V1.Controllers.Mappers
{
    public static class ArticleRequestMappings
    {
        private static readonly DtoModelMapping<ArticleRequest, Article> Mapping =
            new(
                config =>
                {
                    config.ForMember(o => o.StorageLocationId, dest => dest.MapFrom(o => o.StorageLocationId));
                    config.ForMember(o => o.GlobalTradeItemNumber, dest => dest.MapFrom(o => o.GlobalTradeItemNumber));
                    config.ForMember(o => o.Name, dest => dest.MapFrom(o => o.Name));
                    config.ForMember(o => o.BestBeforeDate, dest => dest.MapFrom(o => o.BestBeforeDate));
                    config.ForMember(o => o.Quantity, dest => dest.MapFrom(o => o.Quantity));
                    config.ForMember(o => o.Content, dest => dest.MapFrom(o => o.Content));
                    config.ForMember(o => o.ContentType, dest => dest.MapFrom(o => o.ContentType));
                },
                null);

        public static Article ToModelNotNull(this ArticleRequest dto)
        {
            return Mapping.ToModelNotNull(dto);
        }
    }
}
