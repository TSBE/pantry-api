using Pantry.Core.Mappers;
using Dto = Pantry.Features.WebFeature.V1.Controllers.Enums.DevicePlatformType;
using Persistence = Pantry.Core.Persistence.Enums.DevicePlatformType;

namespace Pantry.Features.WebFeature.V1.Controllers.Mappers;

public static class DevicePlatformTypeMappings
{
    private static readonly EnumDtoModelMapping<Dto, Persistence> Mapping = new();

    public static Persistence ToModelNotNull(this Dto dto)
    {
        return Mapping.ToModelNotNull(dto);
    }

    public static Dto ToDtoNotNull(this Persistence model)
    {
        return Mapping.ToDtoNotNull(model);
    }
}
