#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System.Text.Json.Serialization;
using Pantry.Core.Models.OpenFoodFacts;

namespace Pantry.Features.OpenFoodFacts.Responses;

public class ProductResponse
{
    [JsonPropertyName("product")]
    public Product Product { get; set; }

    [JsonPropertyName("status")]
    public int Status { get; set; }

    [JsonPropertyName("code")]
    public string Code { get; set; }

    [JsonPropertyName("status_verbose")]
    public string StatusVerbose { get; set; }
}
