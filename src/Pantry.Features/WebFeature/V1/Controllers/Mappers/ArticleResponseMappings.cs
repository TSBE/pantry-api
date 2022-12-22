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
                    config.ForMember(o => o.StorageLocation, dest => dest.MapFrom(o => o.StorageLocation.ToDtoNotNull()));
                    config.ForMember(o => o.GlobalTradeItemNumber, dest => dest.MapFrom(o => o.GlobalTradeItemNumber));
                    config.ForMember(o => o.Name, dest => dest.MapFrom(o => o.Name));
                    config.ForMember(o => o.BestBeforeDate, dest => dest.MapFrom(o => o.BestBeforeDate));
                    config.ForMember(o => o.Quantity, dest => dest.MapFrom(o => o.Quantity));
                    config.ForMember(o => o.Content, dest => dest.MapFrom(o => o.Content));
                    config.ForMember(o => o.ContentType, dest => dest.MapFrom(o => o.ContentType));

                    config.ForMember(
                    o => o.Brands,
                    dest => dest.MapFrom((o, d) =>
                    {
                        return o.Metadata?.FoodFacts?.Brands ?? null;
                    }));
                    config.ForMember(
                    o => o.ImageUrl,
                    dest => dest.MapFrom((o, d) =>
                    {
                        return o.ImageUrl ?? o.Metadata?.FoodFacts?.ImageUrl ?? o.Metadata?.FoodFacts?.ImageFrontUrl ?? null;
                    }));
                    config.ForMember(
                    o => o.Nutriments,
                    dest => dest.MapFrom((o, d) =>
                    {
                        var n = o.Metadata?.FoodFacts?.Nutriments;
                        if (n != null)
                        {
                            return new Dictionary<string, NutrimentResponse>
                            {
                                { "Energykcal", new NutrimentResponse { Name = n.Energykcal, Unit = n.EnergykcalUnit, Value = n.EnergykcalValue, ValueFor100g = n.Energykcal100g } },
                                { "Energy", new NutrimentResponse { Name = n.Energy, Unit = n.EnergyUnit, Value = n.EnergyValue, ValueFor100g = n.Energy100g } },
                                { "Fat", new NutrimentResponse { Name = n.Fat, Unit = n.FatUnit, Value = n.FatValue, ValueFor100g = n.Fat100g } },
                                { "SaturadedFat", new NutrimentResponse { Name = n.Saturatedfat, Unit = n.SaturatedfatUnit, Value = n.SaturatedfatValue, ValueFor100g = n.Saturatedfat100g } },
                                { "Carbohydrates", new NutrimentResponse { Name = n.Carbohydrates, Unit = n.CarbohydratesUnit, Value = n.CarbohydratesValue, ValueFor100g = n.Carbohydrates100g } },
                                { "Sugar", new NutrimentResponse { Name = n.Sugars, Unit = n.SugarsUnit, Value = n.SugarsValue, ValueFor100g = n.Sugars100g } },
                                { "Fiber", new NutrimentResponse { Name = n.Fiber, Unit = n.FiberUnit, Value = n.FiberValue, ValueFor100g = n.Fiber100g } },
                                { "Protein", new NutrimentResponse { Name = n.Proteins, Unit = n.ProteinsUnit, Value = n.ProteinsValue, ValueFor100g = n.Proteins100g } },
                                { "Salt", new NutrimentResponse { Name = n.Salt, Unit = n.SaltUnit, Value = n.SaltValue, ValueFor100g = n.Salt100g } },
                                { "Sodium", new NutrimentResponse { Name = n.Sodium, Unit = n.SodiumUnit, Value = n.SodiumValue, ValueFor100g = n.Sodium100g } }
                            };
                        }

                        return null;
                    }));
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
