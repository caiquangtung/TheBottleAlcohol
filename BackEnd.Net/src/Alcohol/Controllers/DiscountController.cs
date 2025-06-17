using System;
using System.Threading.Tasks;
using Alcohol.DTOs.Discount;
using Alcohol.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Alcohol.Common;

namespace Alcohol.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DiscountController : ControllerBase
    {
        private readonly IDiscountService _discountService;

        public DiscountController(IDiscountService discountService)
        {
            _discountService = discountService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var discounts = await _discountService.GetAllDiscountsAsync();
            return Ok(new ApiResponse<IEnumerable<DiscountResponseDto>>(discounts));
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var discounts = await _discountService.GetActiveDiscountsAsync();
            return Ok(new ApiResponse<IEnumerable<DiscountResponseDto>>(discounts));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var discount = await _discountService.GetDiscountByIdAsync(id);
            if (discount == null)
                return NotFound(new ApiResponse<string>("Discount not found"));
            return Ok(new ApiResponse<DiscountResponseDto>(discount));
        }

        [HttpGet("code/{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            var discount = await _discountService.GetDiscountByCodeAsync(code);
            if (discount == null)
                return NotFound(new ApiResponse<string>("Discount not found"));
            return Ok(new ApiResponse<DiscountResponseDto>(discount));
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create(DiscountCreateDto createDto)
        {
            var discount = await _discountService.CreateDiscountAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = discount.Id }, new ApiResponse<DiscountResponseDto>(discount));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Update(int id, DiscountUpdateDto updateDto)
        {
            var discount = await _discountService.UpdateDiscountAsync(id, updateDto);
            if (discount == null)
                return NotFound(new ApiResponse<string>("Discount not found"));
            return Ok(new ApiResponse<DiscountResponseDto>(discount));
        }

        [HttpPut("{id}/toggle")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var result = await _discountService.ToggleDiscountStatusAsync(id);
            if (!result)
                return NotFound(new ApiResponse<string>("Discount not found"));
            return Ok(new ApiResponse<string>("Discount status updated successfully"));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _discountService.DeleteDiscountAsync(id);
            if (!result)
                return NotFound(new ApiResponse<string>("Discount not found"));
            return Ok(new ApiResponse<string>("Discount deleted successfully"));
        }
    }
} 