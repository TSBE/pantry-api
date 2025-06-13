using System;
using System.Collections.Generic;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.V1.Controllers.Responses;

namespace Pantry.Features.WebFeature.V1.Controllers.Extensions;

   public static class MetadataResponseExtensions
    {
        public static MetadataResponse ToDtoNotNull(this Metadata model)
        {
            ArgumentNullException.ThrowIfNull(model);

            var foodFacts = model.FoodFacts;
            var nutriments = foodFacts?.Nutriments;

            return new MetadataResponse
            {
                GlobalTradeItemNumber = model.GlobalTradeItemNumber,
                Name = foodFacts?.ProductNameDe
                    ?? foodFacts?.ProductName
                    ?? model.ProductFacts?.Name,
                Brands = foodFacts?.Brands,
                ImageUrl = foodFacts?.ImageUrl
                    ?? foodFacts?.ImageFrontUrl,
                Nutriments = nutriments != null
                    ? new Dictionary<string, NutrimentResponse>
                    {
                        { "Energykcal", new NutrimentResponse { Name = nutriments.Energykcal, Unit = nutriments.EnergykcalUnit, Value = nutriments.EnergykcalValue, ValueFor100g = nutriments.Energykcal100g } },
                        { "Energy", new NutrimentResponse { Name = nutriments.Energy, Unit = nutriments.EnergyUnit, Value = nutriments.EnergyValue, ValueFor100g = nutriments.Energy100g } },
                        { "Fat", new NutrimentResponse { Name = nutriments.Fat, Unit = nutriments.FatUnit, Value = nutriments.FatValue, ValueFor100g = nutriments.Fat100g } },
                        { "SaturadedFat", new NutrimentResponse { Name = nutriments.Saturatedfat, Unit = nutriments.SaturatedfatUnit, Value = nutriments.SaturatedfatValue, ValueFor100g = nutriments.Saturatedfat100g } },
                        { "Carbohydrates", new NutrimentResponse { Name = nutriments.Carbohydrates, Unit = nutriments.CarbohydratesUnit, Value = nutriments.CarbohydratesValue, ValueFor100g = nutriments.Carbohydrates100g } },
                        { "Sugar", new NutrimentResponse { Name = nutriments.Sugars, Unit = nutriments.SugarsUnit, Value = nutriments.SugarsValue, ValueFor100g = nutriments.Sugars100g } },
                        { "Fiber", new NutrimentResponse { Name = nutriments.Fiber, Unit = nutriments.FiberUnit, Value = nutriments.FiberValue, ValueFor100g = nutriments.Fiber100g } },
                        { "Protein", new NutrimentResponse { Name = nutriments.Proteins, Unit = nutriments.ProteinsUnit, Value = nutriments.ProteinsValue, ValueFor100g = nutriments.Proteins100g } },
                        { "Salt", new NutrimentResponse { Name = nutriments.Salt, Unit = nutriments.SaltUnit, Value = nutriments.SaltValue, ValueFor100g = nutriments.Salt100g } },
                        { "Sodium", new NutrimentResponse { Name = nutriments.Sodium, Unit = nutriments.SodiumUnit, Value = nutriments.SodiumValue, ValueFor100g = nutriments.Sodium100g } }
                    }
                    : null
            };
        }
    }
