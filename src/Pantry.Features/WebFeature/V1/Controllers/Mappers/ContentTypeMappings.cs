using Pantry.Core.Mappers;
using Dto = Pantry.Features.WebFeature.V1.Controllers.Enums.ContentType;
using Persistence = Pantry.Core.Persistence.Enums.ContentType;

namespace Pantry.Features.WebFeature.V1.Controllers.Mappers;

public static class ContentTypeMappings
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
