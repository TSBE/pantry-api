using System;
using AutoMapper.Extensions.EnumMapping;

namespace Pantry.Core.Mappers;

public class EnumDtoModelMapping<TDto, TModel> : DtoModelMapping<TDto, TModel>
    where TDto : struct, Enum
    where TModel : struct, Enum
{
    public EnumDtoModelMapping()
        : base(
            opt => opt.ConvertUsingEnumMapping(enumOpt => enumOpt.MapByName()),
            opt => opt.ConvertUsingEnumMapping(enumOpt => enumOpt.MapByName()))
    {
    }
}
