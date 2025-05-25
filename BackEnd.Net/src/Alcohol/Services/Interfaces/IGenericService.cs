using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Alcohol.Services.Interfaces;

public interface IGenericService<T, TCreateDto, TUpdateDto>
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> GetByIdAsync(int id);
    Task<T> CreateAsync(TCreateDto dto);
    Task<T> UpdateAsync(int id, TUpdateDto dto);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<int> CountAsync();
}

public interface IGenericService<T, TCreateDto, TUpdateDto, TResponseDto> : IGenericService<T, TCreateDto, TUpdateDto>
{
    Task<IEnumerable<TResponseDto>> GetAllResponseAsync();
    Task<TResponseDto> GetResponseByIdAsync(int id);
    Task<TResponseDto> CreateResponseAsync(TCreateDto dto);
    Task<TResponseDto> UpdateResponseAsync(int id, TUpdateDto dto);
} 