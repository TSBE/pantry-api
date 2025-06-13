using Pantry.Core.Persistence.Enums;
using Pantry.Features.WebFeature.V1.Controllers.Enums;

namespace Pantry.Features.WebFeature.V1.Controllers.Extensions;

public static class ContentTypeExtensions
{
    public static ContentType ToModelNotNull(this ContentTypeDto dto)
    {
        return dto switch
        {
            _ => ContentType.UNKNOWN
        };
    }

    public static ContentTypeDto ToDtoNotNull(this ContentType model)
    {
        return model switch
        {
            _ => ContentTypeDto.UNKNOWN
        };
    }
}
