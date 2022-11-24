﻿#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System;
using Pantry.Features.WebFeature.V1.Controllers.Enums;

namespace Pantry.Features.WebFeature.V1.Controllers.Responses;

/// <summary>
/// Represents article.
/// </summary>
public class ArticleResponse
{
    /// <summary>
    /// Represents the database internal id.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Storage location id.
    /// </summary>
    public long StorageLocationId { get; set; }

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
}
