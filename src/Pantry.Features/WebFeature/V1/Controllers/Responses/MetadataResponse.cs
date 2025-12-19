using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Pantry.Features.WebFeature.V1.Controllers.Responses;

/// <summary>
/// Represents article metadata.
/// </summary>
[SuppressMessage("Design", "CA1056:Uri properties should not be strings", Justification = "This is a external api field")]
public class MetadataResponse
{
    /// <summary>
    /// The Global Trade Item Number (GTIN) a.k.a. (EAN).
    /// </summary>
    public string? GlobalTradeItemNumber { get; set; }

    /// <summary>
    /// The name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// The brands.
    /// </summary>
    public string? Brands { get; set; }

    /// <summary>
    /// The image url.
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Nutriments.
    /// </summary>
    public IDictionary<string, NutrimentResponse>? Nutriments { get; set; }
}
