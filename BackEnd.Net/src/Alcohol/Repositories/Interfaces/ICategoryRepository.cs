using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.Models;

namespace Alcohol.Repositories.Interfaces;

public interface ICategoryRepository : IGenericRepository<Category>
{
    Task<IEnumerable<Category>> GetRootCategoriesAsync();
    Task<IEnumerable<Category>> GetSubCategoriesAsync(int parentId);
    Task<Category> GetCategoryWithChildrenAsync(int id);
    Task<Category> GetCategoryWithProductsAsync(int id);
    Task<bool> HasChildrenAsync(int id);
    Task<bool> HasProductsAsync(int id);
} 