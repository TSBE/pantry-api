using System;

namespace Pantry.Features.WebFeature.V1.Controllers.Requests;

/// <summary>
/// Represents an Invitation to a household.
/// </summary>
public class InvitationRequest
{
    /// <summary>
    /// A guid which is the public invate id.
    /// </summary>
    public Guid FriendsCode { get; set; }
}
