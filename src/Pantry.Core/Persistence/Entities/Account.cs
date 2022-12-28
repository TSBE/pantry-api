#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System;
using System.Collections.Generic;

namespace Pantry.Core.Persistence.Entities;

/// <summary>
/// Represents an account that holds user informations.
/// </summary>
public class Account : Auditable
{
    /// <summary>
    /// Represents the database internal id.
    /// </summary>
    public long AccountId { get; set; }

    /// <summary>
    /// The users first name.
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    /// The users last name.
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    /// A guid which is the public invate id.
    /// </summary>
    public Guid FriendsCode { get; set; }

    /// <summary>
    /// Some id from the auth provider.
    /// </summary>
    public string OAuhtId { get; set; }

    public long? HouseholdId { get; set; }

    public virtual Household? Household { get; set; }

    /// <summary>
    /// Navigation to devices.
    /// </summary>
    public virtual ICollection<Device>? Devices { get; private set; }
}
