using System;
using System.Threading.Tasks;
using Alcohol.DTOs.InventoryTransaction;
using Alcohol.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Alcohol.Common;
using Alcohol.DTOs;

namespace Alcohol.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class InventoryTransactionController : ControllerBase
{
    private readonly IInventoryTransactionService _inventoryTransactionService;

    public InventoryTransactionController(IInventoryTransactionService inventoryTransactionService)
    {
        _inventoryTransactionService = inventoryTransactionService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetAllInventoryTransactions([FromQuery] InventoryTransactionFilterDto filter)
    {
        var result = await _inventoryTransactionService.GetAllInventoryTransactionsAsync(filter);
        return Ok(new ApiResponse<PagedResult<InventoryTransactionResponseDto>>(result));
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetInventoryTransactionById(int id)
    {
        var inventoryTransaction = await _inventoryTransactionService.GetInventoryTransactionByIdAsync(id);
        if (inventoryTransaction == null)
            return NotFound(new ApiResponse<string>("Inventory transaction not found"));
        return Ok(new ApiResponse<InventoryTransactionResponseDto>(inventoryTransaction));
    }

    [HttpGet("product/{productId}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetInventoryTransactionsByProduct(int productId)
    {
        var inventoryTransactions = await _inventoryTransactionService.GetInventoryTransactionsByProductAsync(productId);
        return Ok(new ApiResponse<InventoryTransactionResponseDto[]>(inventoryTransactions.ToArray()));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> CreateInventoryTransaction(InventoryTransactionCreateDto createDto)
    {
        try
        {
            var inventoryTransaction = await _inventoryTransactionService.CreateInventoryTransactionAsync(createDto);
            return CreatedAtAction(nameof(GetInventoryTransactionById), new { id = inventoryTransaction.Id }, new ApiResponse<InventoryTransactionResponseDto>(inventoryTransaction));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<string>(ex.Message));
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> UpdateInventoryTransaction(int id, InventoryTransactionUpdateDto updateDto)
    {
        var inventoryTransaction = await _inventoryTransactionService.UpdateInventoryTransactionAsync(id, updateDto);
        if (inventoryTransaction == null)
            return NotFound(new ApiResponse<string>("Inventory transaction not found"));
        return Ok(new ApiResponse<InventoryTransactionResponseDto>(inventoryTransaction));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> DeleteInventoryTransaction(int id)
    {
        var result = await _inventoryTransactionService.DeleteInventoryTransactionAsync(id);
        if (!result)
            return NotFound(new ApiResponse<string>("Inventory transaction not found"));
        return Ok(new ApiResponse<string>("Inventory transaction deleted successfully"));
    }
} 