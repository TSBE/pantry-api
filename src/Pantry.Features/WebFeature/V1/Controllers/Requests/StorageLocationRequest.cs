﻿#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace Pantry.Features.WebFeature.V1.Controllers.Requests;

/// <summary>
/// Represents a storage location for articles.
/// </summary>
public class StorageLocationRequest
{
    /// <summary>
    /// The name of the location.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The description.
    /// </summary>
    public string Description { get; set; }
}
