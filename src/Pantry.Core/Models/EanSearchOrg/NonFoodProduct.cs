#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System.Text.Json.Serialization;

namespace Pantry.Core.Models.EanSearchOrg;

public partial class NonFoodProduct
{
    [JsonPropertyName("ean")]
    public string Ean { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("categoryId")]
    public string CategoryId { get; set; }

    [JsonPropertyName("categoryName")]
    public string CategoryName { get; set; }

    [JsonPropertyName("issuingCountry")]
    public string IssuingCountry { get; set; }
}
