using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.Repositories.Interfaces;
using Alcohol.Services.Helpers;
using Alcohol.Services.Interfaces;
using AutoMapper;

namespace Alcohol.Services.Base;

public abstract class GenericService<T, TCreateDto, TUpdateDto> : IGenericService<T, TCreateDto, TUpdateDto> where T : class
{
    protected readonly IGenericRepository<T> _repository;
    protected readonly IMapper _mapper;

    protected GenericService(IGenericRepository<T> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public virtual async Task<T> GetByIdAsync(int id)
    {
        await ValidationHelper.ValidateEntityExistsAsync(id, _repository);
        return await _repository.GetByIdAsync(id);
    }

    public virtual async Task<T> CreateAsync(TCreateDto dto)
    {
        var entity = _mapper.Map<T>(dto);
        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<T> UpdateAsync(int id, TUpdateDto dto)
    {
        await ValidationHelper.ValidateEntityExistsAsync(id, _repository);
        var entity = await _repository.GetByIdAsync(id);
        _mapper.Map(dto, entity);
        _repository.Update(entity);
        await _repository.SaveChangesAsync();
        return entity;
    }

    public virtual async Task DeleteAsync(int id)
    {
        await ValidationHelper.ValidateEntityExistsAsync(id, _repository);
        var entity = await _repository.GetByIdAsync(id);
        _repository.Delete(entity);
        await _repository.SaveChangesAsync();
    }

    public virtual async Task<bool> ExistsAsync(int id)
    {
        return await _repository.GetByIdAsync(id) != null;
    }

    public virtual async Task<int> CountAsync()
    {
        var entities = await _repository.GetAllAsync();
        return entities.Count();
    }
}

public abstract class GenericService<T, TCreateDto, TUpdateDto, TResponseDto> : 
    GenericService<T, TCreateDto, TUpdateDto>, 
    IGenericService<T, TCreateDto, TUpdateDto, TResponseDto> where T : class
{
    protected GenericService(IGenericRepository<T> repository, IMapper mapper) : base(repository, mapper)
    {
    }

    public virtual async Task<IEnumerable<TResponseDto>> GetAllResponseAsync()
    {
        var entities = await GetAllAsync();
        return _mapper.Map<IEnumerable<TResponseDto>>(entities);
    }

    public virtual async Task<TResponseDto> GetResponseByIdAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        return _mapper.Map<TResponseDto>(entity);
    }

    public virtual async Task<TResponseDto> CreateResponseAsync(TCreateDto dto)
    {
        var entity = await CreateAsync(dto);
        return _mapper.Map<TResponseDto>(entity);
    }

    public virtual async Task<TResponseDto> UpdateResponseAsync(int id, TUpdateDto dto)
    {
        var entity = await UpdateAsync(id, dto);
        return _mapper.Map<TResponseDto>(entity);
    }
} 