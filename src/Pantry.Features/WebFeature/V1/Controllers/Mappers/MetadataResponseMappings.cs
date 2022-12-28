using System.Collections.Generic;
using Pantry.Core.Mappers;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.V1.Controllers.Responses;

namespace Pantry.Features.WebFeature.V1.Controllers.Mappers
{
    public static class MetadataResponseMappings
    {
        private static readonly DtoModelMapping<MetadataResponse, Metadata> Mapping =
            new(
                null,
                config =>
                {
                    config.ForMember(o => o.GlobalTradeItemNumber, dest => dest.MapFrom(o => o.GlobalTradeItemNumber));
                    config.ForMember(
                        o => o.Name,
                        dest => dest.MapFrom((o, d) =>
                        {
                            return o.FoodFacts?.ProductNameDe ?? o.FoodFacts?.ProductName ?? o.ProductFacts?.Name ?? null;
                        }));
                    config.ForMember(
                    o => o.Brands,
                    dest => dest.MapFrom((o, d) =>
                    {
                        return o.FoodFacts?.Brands ?? null;
                    }));
                    config.ForMember(
                    o => o.ImageUrl,
                    dest => dest.MapFrom((o, d) =>
                    {
                        return o.FoodFacts?.ImageUrl ?? o.FoodFacts?.ImageFrontUrl ?? null;
                    }));
                    config.ForMember(
                    o => o.Nutriments,
                    dest => dest.MapFrom((o, d) =>
                    {
                        var n = o.FoodFacts?.Nutriments;
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

        public static MetadataResponse ToDtoNotNull(this Metadata model)
        {
            return Mapping.ToDtoNotNull(model);
        }
    }
}
