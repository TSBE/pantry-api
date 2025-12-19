using System.Diagnostics.CodeAnalysis;
using Pantry.Core.Models.EanSearchOrg;
using Pantry.Core.Models.OpenFoodFacts;

namespace Pantry.Core.Persistence.Entities;

/// <summary>
/// Represents an metadata.
/// </summary>
[SuppressMessage("Design", "CA1056:Uri properties should not be strings", Justification = "This is a external api field")]
public class Metadata : Auditable
{
    /// <summary>
    /// Represents the database internal id.
    /// </summary>
    public long MetadataId { get; set; }

    /// <summary>
    /// The Global Trade Item Number (GTIN) a.k.a. (EAN) of the article.
    /// </summary>
    public required string GlobalTradeItemNumber { get; set; }

    /// <summary>
    /// JSON food facts data.
    /// </summary>
    public Product? FoodFacts { get; set; }

    /// <summary>
    /// JSON product facts data.
    /// </summary>
    public NonFoodProduct? ProductFacts { get; set; }
}
