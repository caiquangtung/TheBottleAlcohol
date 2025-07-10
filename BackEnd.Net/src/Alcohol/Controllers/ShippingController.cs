using System;
using System.Threading.Tasks;
using Alcohol.DTOs.Shipping;
using Alcohol.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Alcohol.Common;
using Alcohol.DTOs;

namespace Alcohol.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ShippingController : ControllerBase
{
    private readonly IShippingService _shippingService;

    public ShippingController(IShippingService shippingService)
    {
        _shippingService = shippingService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetAllShippings([FromQuery] ShippingFilterDto filter)
    {
        var result = await _shippingService.GetAllShippingsAsync(filter);
        return Ok(new ApiResponse<PagedResult<ShippingResponseDto>>(result));
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetShippingById(int id)
    {
        var shipping = await _shippingService.GetShippingByIdAsync(id);
        if (shipping == null)
            return NotFound(new ApiResponse<string>("Shipping not found"));
        return Ok(new ApiResponse<ShippingResponseDto>(shipping));
    }

    [HttpGet("order/{orderId}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetShippingByOrder(int orderId)
    {
        var shipping = await _shippingService.GetShippingByOrderAsync(orderId);
        if (shipping == null)
            return NotFound(new ApiResponse<string>("Shipping not found"));
        return Ok(new ApiResponse<ShippingResponseDto>(shipping));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> CreateShipping(ShippingCreateDto createDto)
    {
        try
        {
            var shipping = await _shippingService.CreateShippingAsync(createDto);
            return CreatedAtAction(nameof(GetShippingById), new { id = shipping.Id }, new ApiResponse<ShippingResponseDto>(shipping));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<string>(ex.Message));
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> UpdateShipping(int id, ShippingUpdateDto updateDto)
    {
        var shipping = await _shippingService.UpdateShippingAsync(id, updateDto);
        if (shipping == null)
            return NotFound(new ApiResponse<string>("Shipping not found"));
        return Ok(new ApiResponse<ShippingResponseDto>(shipping));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> DeleteShipping(int id)
    {
        var result = await _shippingService.DeleteShippingAsync(id);
        if (!result)
            return NotFound(new ApiResponse<string>("Shipping not found"));
        return Ok(new ApiResponse<string>("Shipping deleted successfully"));
    }
} 