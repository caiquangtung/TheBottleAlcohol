using System;
using Alcohol.Models;

namespace Alcohol.Repositories.IRepositories;

public interface IProductCategoryRepository
{
    Task<List<Category>> GetAllAsync();
    Task<Category> GetByIdAsync(int id);
    Task AddAsync(Category category);
    Task UpdateAsync(Category category);
    Task DeleteAsync(int id);
} 