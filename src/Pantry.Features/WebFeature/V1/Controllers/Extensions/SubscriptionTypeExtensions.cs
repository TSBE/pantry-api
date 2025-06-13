using Pantry.Core.Persistence.Enums;
using Pantry.Features.WebFeature.V1.Controllers.Enums;

namespace Pantry.Features.WebFeature.V1.Controllers.Extensions;

public static class SubscriptionTypeExtensions
{
    public static SubscriptionType ToModelNotNull(this SubscriptionTypeDto dto)
    {
        return dto switch
        {
            SubscriptionTypeDto.FREE => SubscriptionType.FREE,
            SubscriptionTypeDto.PREMIUM => SubscriptionType.PREMIUM,
            SubscriptionTypeDto.FAMILY => SubscriptionType.FAMILY,
            _ => SubscriptionType.UNKNOWN
        };
    }

    public static SubscriptionTypeDto ToDtoNotNull(this SubscriptionType model)
    {
        return model switch
        {
            SubscriptionType.FREE => SubscriptionTypeDto.FREE,
            SubscriptionType.PREMIUM => SubscriptionTypeDto.PREMIUM,
            SubscriptionType.FAMILY => SubscriptionTypeDto.FAMILY,
            _ => SubscriptionTypeDto.UNKNOWN
        };
    }
}
