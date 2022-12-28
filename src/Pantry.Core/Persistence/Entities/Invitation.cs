#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System;

namespace Pantry.Core.Persistence.Entities;

/// <summary>
/// Represents an Invitation to a household.
/// </summary>
public class Invitation : Auditable
{
    /// <summary>
    /// Represents the database internal id.
    /// </summary>
    public long InvitationId { get; set; }

    /// <summary>
    /// Invitation valid until.
    /// </summary>
    public DateTime ValidUntilDate { get; set; }

    /// <summary>
    /// A guid which is the public invate id.
    /// </summary>
    public Guid FriendsCode { get; set; }

    /// <summary>
    /// The creator of the invitation.
    /// </summary>
    public long CreatorId { get; set; }

    /// <summary>
    /// The creator of the invitation.
    /// </summary>
    public virtual Account Creator { get; set; }

    public virtual long HouseholdId { get; set; }

    /// <summary>
    /// The household to which this invitation belongs.
    /// </summary>
    public virtual Household Household { get; set; }
}
