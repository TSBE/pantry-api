using Pantry.Core.Mappers;
using DtoDevicePlatformType = Pantry.Features.WebFeature.V1.Controllers.Enums.DevicePlatformType;
using PersistenceReportStatusType = Pantry.Core.Persistence.Enums.DevicePlatformType;

namespace Pantry.Features.WebFeature.V1.Controllers.Mappers;

public static class DevicePlatformTypeMappings
{
    private static readonly EnumDtoModelMapping<DtoDevicePlatformType, PersistenceReportStatusType> Mapping = new();

    public static PersistenceReportStatusType ToModelNotNull(this DtoDevicePlatformType dto)
    {
        return Mapping.ToModelNotNull(dto);
    }

    public static DtoDevicePlatformType ToDtoNotNull(this PersistenceReportStatusType model)
    {
        return Mapping.ToDtoNotNull(model);
    }
}
