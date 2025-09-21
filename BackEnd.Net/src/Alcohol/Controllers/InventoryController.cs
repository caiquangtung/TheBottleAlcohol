using System;
using System.Collections.Generic;
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

    [HttpPatch("{id}/stock")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> UpdateStock(int id, [FromBody] UpdateStockRequest request)
    {
        try
        {
            var result = await _inventoryService.UpdateStockAsync(id, request.Quantity);
            if (!result)
                return NotFound(new ApiResponse<string>("Inventory not found"));
            return Ok(new ApiResponse<string>("Stock updated successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<string>(ex.Message));
        }
    }

    [HttpPatch("product/{productId}/adjust")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> AdjustStock(int productId, [FromBody] AdjustStockRequest request)
    {
        try
        {
            await _inventoryService.AdjustStockAsync(productId, request.Quantity, request.Notes);
            return Ok(new ApiResponse<string>("Stock adjusted successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<string>(ex.Message));
        }
    }

    [HttpPost("recalculate-values")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RecalculateAllTotalValues()
    {
        try
        {
            await _inventoryService.RecalculateAllTotalValuesAsync();
            return Ok(new ApiResponse<string>("All inventory total values recalculated successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<string>(ex.Message));
        }
    }

    [HttpGet("total-value")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetTotalInventoryValue()
    {
        var totalValue = await _inventoryService.GetTotalInventoryValueAsync();
        return Ok(new ApiResponse<decimal>(totalValue));
    }

    [HttpGet("low-stock")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetLowStockItems()
    {
        var lowStockItems = await _inventoryService.GetLowStockItemsAsync();
        return Ok(new ApiResponse<List<InventoryResponseDto>>(lowStockItems));
    }
} 