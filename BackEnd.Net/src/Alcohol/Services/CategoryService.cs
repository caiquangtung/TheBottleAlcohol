using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.DTOs.Category;
using Alcohol.Models;
using Alcohol.Repositories.Interfaces;
using Alcohol.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Alcohol.DTOs;

namespace Alcohol.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<PagedResult<CategoryResponseDto>> GetAllCategoriesAsync(CategoryFilterDto filter)
        {
            var categories = await _categoryRepository.GetAllAsync();
            
            // Apply filters
            var filteredCategories = categories.AsQueryable();
            
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                filteredCategories = filteredCategories.Where(c => 
                    c.Name.Contains(filter.SearchTerm) || 
                    (c.Description != null && c.Description.Contains(filter.SearchTerm)));
            }
            
            if (filter.ParentId.HasValue)
            {
                filteredCategories = filteredCategories.Where(c => c.ParentId == filter.ParentId.Value);
            }
            
            if (filter.IsActive.HasValue)
            {
                filteredCategories = filteredCategories.Where(c => c.IsActive == filter.IsActive.Value);
            }
            
            // Apply sorting
            if (!string.IsNullOrWhiteSpace(filter.SortBy))
            {
                filteredCategories = filter.SortBy.ToLower() switch
                {
                    "name" => filter.SortOrder?.ToLower() == "desc" 
                        ? filteredCategories.OrderByDescending(c => c.Name)
                        : filteredCategories.OrderBy(c => c.Name),
                    "displayorder" => filter.SortOrder?.ToLower() == "desc"
                        ? filteredCategories.OrderByDescending(c => c.DisplayOrder)
                        : filteredCategories.OrderBy(c => c.DisplayOrder),
                    "createdat" => filter.SortOrder?.ToLower() == "desc"
                        ? filteredCategories.OrderByDescending(c => c.CreatedAt)
                        : filteredCategories.OrderBy(c => c.CreatedAt),
                    _ => filteredCategories.OrderBy(c => c.Id)
                };
            }
            else
            {
                filteredCategories = filteredCategories.OrderBy(c => c.DisplayOrder).ThenBy(c => c.Id);
            }
            
            var totalRecords = filteredCategories.Count();
            var pagedCategories = filteredCategories
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();
            
            var categoryDtos = _mapper.Map<List<CategoryResponseDto>>(pagedCategories);
            return new PagedResult<CategoryResponseDto>(categoryDtos, totalRecords, filter.PageNumber, filter.PageSize);
        }

        public async Task<IEnumerable<CategoryResponseDto>> GetRootCategoriesAsync()
        {
            var categories = await _categoryRepository.GetRootCategoriesAsync();
            return _mapper.Map<IEnumerable<CategoryResponseDto>>(categories);
        }

        public async Task<IEnumerable<CategoryResponseDto>> GetSubCategoriesAsync(int parentId)
        {
            var categories = await _categoryRepository.GetSubCategoriesAsync(parentId);
            return _mapper.Map<IEnumerable<CategoryResponseDto>>(categories);
        }

        public async Task<CategoryResponseDto> GetCategoryByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdWithParentAsync(id);
            if (category == null)
                return null;

            return _mapper.Map<CategoryResponseDto>(category);
        }

        public async Task<CategoryResponseDto> GetCategoryWithChildrenAsync(int id)
        {
            var category = await _categoryRepository.GetCategoryWithChildrenAsync(id);
            if (category == null)
                return null;

            return _mapper.Map<CategoryResponseDto>(category);
        }

        public async Task<CategoryResponseDto> GetCategoryWithProductsAsync(int id)
        {
            var category = await _categoryRepository.GetCategoryWithProductsAsync(id);
            if (category == null)
                return null;

            return _mapper.Map<CategoryResponseDto>(category);
        }

        public async Task<CategoryResponseDto> CreateCategoryAsync(CategoryCreateDto createDto)
        {
            var category = _mapper.Map<Category>(createDto);
            category.CreatedAt = DateTime.UtcNow;
            category.Slug = GenerateSlug(category.Name);

            await _categoryRepository.AddAsync(category);
            await _categoryRepository.SaveChangesAsync();

            return _mapper.Map<CategoryResponseDto>(category);
        }

        public async Task<CategoryResponseDto> UpdateCategoryAsync(int id, CategoryUpdateDto updateDto)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return null;

            _mapper.Map(updateDto, category);
            category.UpdatedAt = DateTime.UtcNow;
            category.Slug = GenerateSlug(category.Name);

            _categoryRepository.Update(category);
            await _categoryRepository.SaveChangesAsync();

            return _mapper.Map<CategoryResponseDto>(category);
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return false;

            if (await _categoryRepository.HasChildrenAsync(id) || await _categoryRepository.HasProductsAsync(id))
                return false;

            _categoryRepository.Delete(category);
            await _categoryRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> HasChildrenAsync(int id)
        {
            return await _categoryRepository.HasChildrenAsync(id);
        }

        public async Task<bool> HasProductsAsync(int id)
        {
            return await _categoryRepository.HasProductsAsync(id);
        }

        private string GenerateSlug(string name)
        {
            return name.ToLower()
                .Replace(" ", "-")
                .Replace("&", "and")
                .Replace("'", "")
                .Replace("\"", "")
                .Replace("+", "plus")
                .Replace("#", "sharp");
        }
    }
} 