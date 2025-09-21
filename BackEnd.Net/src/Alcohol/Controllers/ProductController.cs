using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.DTOs.Product;
using Alcohol.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Alcohol.Common;
using Alcohol.DTOs;

namespace Alcohol.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _service;
        private readonly IProductDiscountService _productDiscountService;

        public ProductController(IProductService service, IProductDiscountService productDiscountService)
        {
            _service = service;
            _productDiscountService = productDiscountService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ProductFilterDto filter)
        {
            var result = await _service.GetAllProductsAsync(filter);
            return Ok(new ApiResponse<PagedResult<ProductResponseDto>>(result));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var product = await _service.GetProductByIdAsync(id);
                if (product == null)
                    return NotFound(new ApiResponse<string>("Product not found"));
                return Ok(new ApiResponse<ProductResponseDto>(product));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>(ex.Message));
            }
        }

        [HttpPost]
        [Authorize(Roles = "CEO,Manager")]
        public async Task<IActionResult> Create(ProductCreateDto productDto)
        {
            try
            {
                var product = await _service.CreateProductAsync(productDto);
                return CreatedAtAction(nameof(GetById), new { id = product.Id }, 
                    new ApiResponse<ProductResponseDto>(product, "Product created successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>(ex.Message));
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "CEO,Manager")]
        public async Task<IActionResult> Update(int id, ProductUpdateDto productDto)
        {
            try
            {
                var product = await _service.UpdateProductAsync(id, productDto);
                if (product == null)
                    return NotFound(new ApiResponse<string>("Product not found"));
                return Ok(new ApiResponse<ProductResponseDto>(product));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>(ex.Message));
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "CEO,Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _service.DeleteProductAsync(id);
                if (!result)
                    return NotFound(new ApiResponse<string>("Product not found"));
                return Ok(new ApiResponse<string>("Product deleted successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>(ex.Message));
            }
        }

        [HttpPost("list-by-ids")]
        public async Task<IActionResult> GetProductsByIds([FromBody] List<int> ids)
        {
            var products = await _service.GetProductsByIdsAsync(ids);
            return Ok(new ApiResponse<List<ProductResponseDto>>(products));
        }

        [HttpGet("brand/{brandId}")]
        public async Task<IActionResult> GetByBrand(int brandId)
        {
            var products = await _service.GetProductsByBrandAsync(brandId);
            return Ok(new ApiResponse<IEnumerable<ProductResponseDto>>(products));
        }

        [HttpGet("{id}/with-discount")]
        public async Task<IActionResult> GetProductWithDiscount(int id)
        {
            try
            {
                var product = await _productDiscountService.GetProductWithDiscountAsync(id);
                if (product == null)
                    return NotFound(new ApiResponse<string>("Product not found"));
                return Ok(new ApiResponse<ProductResponseDto>(product));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>(ex.Message));
            }
        }

        [HttpGet("with-discounts")]
        public async Task<IActionResult> GetProductsWithDiscounts()
        {
            try
            {
                var products = await _productDiscountService.GetProductsWithDiscountAsync();
                return Ok(new ApiResponse<List<ProductResponseDto>>(products));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>(ex.Message));
            }
        }

        [HttpGet("{id}/discount-price")]
        public async Task<IActionResult> GetDiscountedPrice(int id)
        {
            try
            {
                var discountedPrice = await _productDiscountService.CalculateDiscountedPriceAsync(id);
                return Ok(new ApiResponse<decimal>(discountedPrice));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>(ex.Message));
            }
        }

        [HttpGet("{id}/active-discounts")]
        public async Task<IActionResult> GetActiveDiscounts(int id)
        {
            try
            {
                var discounts = await _productDiscountService.GetActiveDiscountsForProductAsync(id);
                return Ok(new ApiResponse<List<DTOs.Discount.DiscountResponseDto>>(discounts));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>(ex.Message));
            }
        }
    }
} 