#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System.Collections.Generic;

namespace Pantry.Core.Persistence.Entities;

/// <summary>
/// Represents a storage location for articles.
/// </summary>
public class StorageLocation : Auditable
{
    /// <summary>
    /// Represents the database internal id.
    /// </summary>
    public long StorageLocationId { get; set; }

    /// <summary>
    /// The name of the location.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The description.
    /// </summary>
    public string? Description { get; set; }

    public long HouseholdId { get; set; }

    public virtual Household Household { get; set; }

    /// <summary>
    /// Navigation to storage location.
    /// </summary>
    public virtual ICollection<Article>? Articles { get; private set; }
}
