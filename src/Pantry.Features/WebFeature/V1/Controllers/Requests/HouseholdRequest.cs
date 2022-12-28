#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Pantry.Features.WebFeature.V1.Controllers.Enums;

namespace Pantry.Features.WebFeature.V1.Controllers.Requests;

/// <summary>
/// Represents a household.
/// </summary>
public class HouseholdRequest
{
    /// <summary>
    /// The name of the household.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The Subscription type.
    /// </summary>
    public SubscriptionType SubscriptionType { get; set; }
}
