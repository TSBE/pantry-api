using System;
using System.Collections.Generic;
using System.Linq;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.V1.Controllers.Responses;

namespace Pantry.Features.WebFeature.V1.Controllers.Extensions;

public static class ArticleResponseExtensions
    {
        public static ArticleResponse ToDtoNotNull(this Article model)
        {
            ArgumentNullException.ThrowIfNull(model);

            return new ArticleResponse
            {
                Id = model.ArticleId,
                StorageLocation = model.StorageLocation.ToDtoNotNull(),
                GlobalTradeItemNumber = model.GlobalTradeItemNumber,
                Name = model.Name,
                BestBeforeDate = model.BestBeforeDate,
                Quantity = model.Quantity,
                Content = model.Content,
                ContentType = model.ContentType.ToDtoNotNull(),
                Brands = model.Metadata?.FoodFacts?.Brands,
                ImageUrl = model.ImageUrl
                    ?? model.Metadata?.FoodFacts?.ImageUrl
                    ?? model.Metadata?.FoodFacts?.ImageFrontUrl,
                Nutriments = model.Metadata?.FoodFacts?.Nutriments is { } n
                    ? new Dictionary<string, NutrimentResponse>
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
                    }
                    : null
            };
        }

        public static ArticleResponse? ToDto(this Article? model)
        {
            return model?.ToDtoNotNull();
        }

        public static ArticleResponse[] ToDtos(this IEnumerable<Article> models)
        {
        ArgumentNullException.ThrowIfNull(models);
        return models.Select(m => m.ToDtoNotNull()).ToArray();
        }
    }
