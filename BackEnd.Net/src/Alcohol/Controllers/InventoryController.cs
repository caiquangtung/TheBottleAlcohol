using System;
using System.Threading.Tasks;
using Alcohol.DTOs.Inventory;
using Alcohol.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Alcohol.Common;
using Alcohol.DTOs;

namespace Alcohol.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetAllInventories([FromQuery] InventoryFilterDto filter)
    {
        var result = await _inventoryService.GetAllInventoriesAsync(filter);
        return Ok(new ApiResponse<PagedResult<InventoryResponseDto>>(result));
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetInventoryById(int id)
    {
        var inventory = await _inventoryService.GetInventoryByIdAsync(id);
        if (inventory == null)
            return NotFound(new ApiResponse<string>("Inventory not found"));
        return Ok(new ApiResponse<InventoryResponseDto>(inventory));
    }

    [HttpGet("product/{productId}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetInventoryByProduct(int productId)
    {
        var inventory = await _inventoryService.GetInventoryByProductAsync(productId);
        if (inventory == null)
            return NotFound(new ApiResponse<string>("Inventory not found"));
        return Ok(new ApiResponse<InventoryResponseDto>(inventory));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> CreateInventory(InventoryCreateDto createDto)
    {
        try
        {
            var inventory = await _inventoryService.CreateInventoryAsync(createDto);
            return CreatedAtAction(nameof(GetInventoryById), new { id = inventory.Id }, new ApiResponse<InventoryResponseDto>(inventory));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<string>(ex.Message));
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> UpdateInventory(int id, InventoryUpdateDto updateDto)
    {
        var inventory = await _inventoryService.UpdateInventoryAsync(id, updateDto);
        if (inventory == null)
            return NotFound(new ApiResponse<string>("Inventory not found"));
        return Ok(new ApiResponse<InventoryResponseDto>(inventory));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> DeleteInventory(int id)
    {
        var result = await _inventoryService.DeleteInventoryAsync(id);
        if (!result)
            return NotFound(new ApiResponse<string>("Inventory not found"));
        return Ok(new ApiResponse<string>("Inventory deleted successfully"));
    }
} 