using Alcohol.DTOs.Category;
using Alcohol.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Alcohol.Common;
using Alcohol.DTOs;

namespace Alcohol.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories([FromQuery] CategoryFilterDto filter)
        {
            var result = await _categoryService.GetAllCategoriesAsync(filter);
            return Ok(new ApiResponse<PagedResult<CategoryResponseDto>>(result));
        }

        [HttpGet("root")]
        public async Task<IActionResult> GetRootCategories()
        {
            var categories = await _categoryService.GetRootCategoriesAsync();
            return Ok(new ApiResponse<IEnumerable<CategoryResponseDto>>(categories));
        }

        [HttpGet("sub/{parentId}")]
        public async Task<IActionResult> GetSubCategories(int parentId)
        {
            var categories = await _categoryService.GetSubCategoriesAsync(parentId);
            return Ok(new ApiResponse<IEnumerable<CategoryResponseDto>>(categories));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
                return NotFound(new ApiResponse<string>("Category not found"));
            return Ok(new ApiResponse<CategoryResponseDto>(category));
        }

        [Authorize(Roles = "CEO,Manager")]
        [HttpPost]
        public async Task<IActionResult> CreateCategory(CategoryCreateDto categoryDto)
        {
            var category = await _categoryService.CreateCategoryAsync(categoryDto);
            return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, new ApiResponse<CategoryResponseDto>(category));
        }

        [Authorize(Roles = "CEO,Manager")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, CategoryUpdateDto categoryDto)
        {
            var category = await _categoryService.UpdateCategoryAsync(id, categoryDto);
            if (category == null)
                return NotFound(new ApiResponse<string>("Category not found"));
            return Ok(new ApiResponse<CategoryResponseDto>(category));
        }

        [Authorize(Roles = "CEO,Manager")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var result = await _categoryService.DeleteCategoryAsync(id);
                if (!result)
                    return NotFound(new ApiResponse<string>("Category not found or has children/products"));
                return Ok(new ApiResponse<string>("Category deleted successfully"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<string>(ex.Message));
            }
        }
    }
} 