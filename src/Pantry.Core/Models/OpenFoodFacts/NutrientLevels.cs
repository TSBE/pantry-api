using System.Text.Json.Serialization;

namespace Pantry.Core.Models.OpenFoodFacts;

public class NutrientLevels
{
    [JsonPropertyName("salt")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public NutrimentLevelType Salt { get; set; }

    [JsonPropertyName("fat")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public NutrimentLevelType Fat { get; set; }

    [JsonPropertyName("sugars")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public NutrimentLevelType Sugars { get; set; }

    [JsonPropertyName("saturated-fat")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public NutrimentLevelType SaturatedFat { get; set; }
}
