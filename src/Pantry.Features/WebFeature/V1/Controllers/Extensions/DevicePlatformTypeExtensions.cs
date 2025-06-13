using Pantry.Core.Persistence.Enums;
using Pantry.Features.WebFeature.V1.Controllers.Enums;

namespace Pantry.Features.WebFeature.V1.Controllers.Extensions;

public static class DevicePlatformTypeExtensions
{
    public static DevicePlatformType ToModelNotNull(this DevicePlatformTypeDto dto)
    {
        return dto switch
        {
            DevicePlatformTypeDto.ANDROID => DevicePlatformType.ANDROID,
            DevicePlatformTypeDto.IOS => DevicePlatformType.IOS,
            _ => DevicePlatformType.UNKNOWN
        };
    }

    public static DevicePlatformTypeDto ToDtoNotNull(this DevicePlatformType model)
    {
        return model switch
        {
            DevicePlatformType.ANDROID => DevicePlatformTypeDto.ANDROID,
            DevicePlatformType.IOS => DevicePlatformTypeDto.IOS,
            _ => DevicePlatformTypeDto.UNKNOWN
        };
    }
}
