#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System;
using Pantry.Core.Persistence.Enums;

namespace Pantry.Core.Persistence.Entities;

/// <summary>
/// Represents article.
/// </summary>
public class Article : Auditable
{
    /// <summary>
    /// Represents the database internal id.
    /// </summary>
    public long ArticleId { get; set; }

    /// <summary>
    /// The Global Trade Item Number (GTIN) a.k.a. (EAN) of the article.
    /// </summary>
    public string? GlobalTradeItemNumber { get; set; }

    /// <summary>
    /// The name of the article.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The best before date.
    /// </summary>
    public DateTime BestBeforeDate { get; set; }

    /// <summary>
    /// The quantity article.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// The content of the article.
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// The content type of the article.
    /// </summary>
    public ContentType ContentType { get; set; }

    /// <summary>
    /// The image url of the article.
    /// </summary>
#pragma warning disable CA1056 // URI-like properties should not be strings
    public string? ImageUrl { get; set; }
#pragma warning restore CA1056 // URI-like properties should not be strings

    public Metadata? Metadata { get; set; }

    public long StorageLocationId { get; set; }

    public virtual StorageLocation StorageLocation { get; set; }

    public long HouseholdId { get; set; }

    public virtual Household Household { get; set; }
}
