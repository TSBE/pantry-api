using System;
using System.Collections.Generic;
using AutoMapper;

namespace Pantry.Core.Mappers;

public class DtoModelMapping<TDto, TModel>
{
    public DtoModelMapping(Action<IMappingExpression<TDto, TModel>>? dtoToModel = null, Action<IMappingExpression<TModel, TDto>>? modelToDtoModel = null)
    {
        DtoToModelMapper = dtoToModel != null
            ? new MapperConfiguration(cfg => dtoToModel(cfg.CreateMap<TDto, TModel>())).CreateMapper()
            : new MapperConfiguration(cfg => cfg.CreateMap<TDto, TModel>()).CreateMapper();
        ModelToDtoMapper = modelToDtoModel != null
            ? new MapperConfiguration(cfg => modelToDtoModel(cfg.CreateMap<TModel, TDto>())).CreateMapper()
            : new MapperConfiguration(cfg => cfg.CreateMap<TModel, TDto>()).CreateMapper();
    }

    protected IMapper DtoToModelMapper { get; }

    protected IMapper ModelToDtoMapper { get; }

    public TDto? ToDto(TModel model)
    {
        return EqualityComparer<TModel>.Default.Equals(model, default) ? default : ModelToDtoMapper.Map<TModel, TDto>(model);
    }

    public TDto ToDtoNotNull(TModel model)
    {
        return ModelToDtoMapper.Map<TModel, TDto>(model);
    }

    public TDto? ToDto(TModel model, Action<IMappingOperationOptions> opts)
    {
        return EqualityComparer<TModel>.Default.Equals(model, default) ? default : ModelToDtoMapper.Map<TModel, TDto>(model, opts);
    }

    public TDto[] ToDtos(IEnumerable<TModel>? models)
    {
        return ModelToDtoMapper.Map<IEnumerable<TModel>, TDto[]>(models ?? Array.Empty<TModel>());
    }

    public TDto[] ToDtos(IEnumerable<TModel>? models, Action<IMappingOperationOptions> opts)
    {
        return ModelToDtoMapper.Map<IEnumerable<TModel>, TDto[]>(models ?? Array.Empty<TModel>(), opts);
    }

    public TModel? ToModel(TDto dto)
    {
        return EqualityComparer<TDto>.Default.Equals(dto, default) ? default : DtoToModelMapper.Map<TDto, TModel>(dto);
    }

    public TModel ToModelNotNull(TDto dto)
    {
        return DtoToModelMapper.Map<TDto, TModel>(dto);
    }

    public TModel? ToModel(TDto dto, Action<IMappingOperationOptions> opts)
    {
        return EqualityComparer<TDto>.Default.Equals(dto, default) ? default : DtoToModelMapper.Map<TDto, TModel>(dto, opts);
    }

    public TModel[] ToModels(IEnumerable<TDto>? dtos)
    {
        return DtoToModelMapper.Map<IEnumerable<TDto>, TModel[]>(dtos ?? Array.Empty<TDto>());
    }

    public TModel[] ToModels(IEnumerable<TDto>? dtos, Action<IMappingOperationOptions> opts)
    {
        return DtoToModelMapper.Map<IEnumerable<TDto>, TModel[]>(dtos ?? Array.Empty<TDto>(), opts);
    }
}
