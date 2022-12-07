#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Pantry.Core.Models.OpenFoodFacts;

public class Product
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("product_name")]
    public string ProductName { get; set; }

    [JsonPropertyName("product_name_de")]
    public string ProductNameDe { get; set; }

    [JsonPropertyName("generic_name")]
    public string GenericName { get; set; }

    [JsonPropertyName("product_quantity")]
    public string ProductQuantity { get; set; }

    [JsonPropertyName("quantity")]
    public string Quantity { get; set; }

    [JsonPropertyName("nutrient_levels")]
    public NutrientLevels NutrientLevels { get; set; }

    [JsonPropertyName("nutriments")]
    public Nutriments Nutriments { get; set; }

    [JsonPropertyName("brands")]
    public string Brands { get; set; }

    [JsonPropertyName("brands_tags")]
    public IEnumerable<string> BrandsTags { get; set; }

    [JsonPropertyName("nova_group")]
    public int NovaGroup { get; set; }

    [JsonPropertyName("image_front_small_url")]
    public string ImageFrontSmallUrl { get; set; }

    [JsonPropertyName("image_front_thumb_url")]
    public string ImageFrontThumbUrl { get; set; }

    [JsonPropertyName("image_front_url")]
    public string ImageFrontUrl { get; set; }

    [JsonPropertyName("image_small_url")]
    public string ImageSmallUrl { get; set; }

    [JsonPropertyName("image_thumb_url")]
    public string ImageThumbUrl { get; set; }

    [JsonPropertyName("image_url")]
    public string ImageUrl { get; set; }

    [JsonPropertyName("created_t")]
    public DateTime CreatedDateTime { get; set; }

    [JsonPropertyName("last_modified_t")]
    public DateTime LastModifiedTime { get; set; }
}
