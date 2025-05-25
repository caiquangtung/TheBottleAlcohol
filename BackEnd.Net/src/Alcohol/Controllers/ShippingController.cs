using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.DTOs.Shipping;
using Alcohol.Models.Enums;
using Alcohol.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Alcohol.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShippingController : ControllerBase
{
    private readonly IShippingService _shippingService;

    public ShippingController(IShippingService shippingService)
    {
        _shippingService = shippingService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ShippingResponseDto>>> GetAllShippings()
    {
        var shippings = await _shippingService.GetAllShippingsAsync();
        return Ok(shippings);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ShippingResponseDto>> GetShippingById(int id)
    {
        var shipping = await _shippingService.GetShippingByIdAsync(id);
        if (shipping == null)
            return NotFound();

        return Ok(shipping);
    }

    [HttpGet("order/{orderId}")]
    public async Task<ActionResult<IEnumerable<ShippingResponseDto>>> GetShippingsByOrder(int orderId)
    {
        var shippings = await _shippingService.GetShippingsByOrderAsync(orderId);
        return Ok(shippings);
    }

    [HttpGet("customer/{customerId}")]
    public async Task<ActionResult<IEnumerable<ShippingResponseDto>>> GetShippingsByCustomer(int customerId)
    {
        var shippings = await _shippingService.GetShippingsByCustomerAsync(customerId);
        return Ok(shippings);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ShippingResponseDto>> CreateShipping(ShippingCreateDto createDto)
    {
        var shipping = await _shippingService.CreateShippingAsync(createDto);
        return CreatedAtAction(nameof(GetShippingById), new { id = shipping.Id }, shipping);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<ShippingResponseDto>> UpdateShipping(int id, ShippingUpdateDto updateDto)
    {
        var shipping = await _shippingService.UpdateShippingAsync(id, updateDto);
        if (shipping == null)
            return NotFound();

        return Ok(shipping);
    }

    [HttpPut("{id}/status")]
    [Authorize]
    public async Task<ActionResult<ShippingResponseDto>> UpdateShippingStatus(int id, ShippingStatusType status)
    {
        var shipping = await _shippingService.UpdateShippingStatusAsync(id, status);
        if (shipping == false)
            return NotFound();

        return Ok(shipping);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult> DeleteShipping(int id)
    {
        var result = await _shippingService.DeleteShippingAsync(id);
        if (result == false)
            return NotFound();

        return NoContent();
    }
} 