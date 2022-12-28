#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System.Collections.Generic;
using Pantry.Core.Persistence.Enums;

namespace Pantry.Core.Persistence.Entities;

/// <summary>
/// Represents a household.
/// </summary>
public class Household : Auditable
{
    /// <summary>
    /// Represents the database internal id.
    /// </summary>
    public long HouseholdId { get; set; }

    /// <summary>
    /// The name of the household.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The Subscription type.
    /// </summary>
    public SubscriptionType SubscriptionType { get; set; }

    /// <summary>
    /// Stores the owner loosely coupled.
    /// </summary>
    public virtual long OwnerId { get; set; }

    /// <summary>
    /// Navigation to invitations.
    /// </summary>
    public virtual ICollection<Invitation>? Invitations { get; private set; }

    /// <summary>
    /// Navigation to storage locations.
    /// </summary>
    public virtual ICollection<StorageLocation>? StorageLocations { get; private set; }

    ///// <summary>
    ///// Navigation to articles.
    ///// </summary>
    public virtual ICollection<Article>? Articles { get; private set; }

    /// <summary>
    /// Navigation to accounts.
    /// </summary>
    public virtual ICollection<Account>? Accounts { get; private set; }
}
