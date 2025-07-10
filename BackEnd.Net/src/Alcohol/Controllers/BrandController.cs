using System;
using System.Threading.Tasks;
using Alcohol.DTOs.Brand;
using Alcohol.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Alcohol.Common;
using Alcohol.DTOs;

namespace Alcohol.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private readonly IBrandService _brandService;

        public BrandController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] BrandFilterDto filter)
        {
            var result = await _brandService.GetAllBrandsAsync(filter);
            return Ok(new ApiResponse<PagedResult<BrandResponseDto>>(result));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var brand = await _brandService.GetBrandByIdAsync(id);
            if (brand == null)
                return NotFound(new ApiResponse<string>("Brand not found"));
            return Ok(new ApiResponse<BrandResponseDto>(brand));
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var brands = await _brandService.GetActiveBrandsAsync();
            return Ok(new ApiResponse<IEnumerable<BrandResponseDto>>(brands));
        }

        [HttpGet("{id}/products")]
        public async Task<IActionResult> GetWithProducts(int id)
        {
            var brand = await _brandService.GetBrandWithProductsAsync(id);
            if (brand == null)
                return NotFound(new ApiResponse<string>("Brand not found"));
            return Ok(new ApiResponse<BrandResponseDto>(brand));
        }

        [HttpPost]
        [Authorize(Roles = "CEO,Manager")]
        public async Task<IActionResult> Create(BrandCreateDto createDto)
        {
            var brand = await _brandService.CreateBrandAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = brand.Id }, new ApiResponse<BrandResponseDto>(brand));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "CEO,Manager")]
        public async Task<IActionResult> Update(int id, BrandUpdateDto updateDto)
        {
            var brand = await _brandService.UpdateBrandAsync(id, updateDto);
            if (brand == null)
                return NotFound(new ApiResponse<string>("Brand not found"));
            return Ok(new ApiResponse<BrandResponseDto>(brand));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "CEO,Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _brandService.DeleteBrandAsync(id);
            if (!result)
                return NotFound(new ApiResponse<string>("Brand not found"));
            return Ok(new ApiResponse<string>("Brand deleted successfully"));
        }
    }
} 