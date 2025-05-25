using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.DTOs.InventoryTransaction;
using Alcohol.Models.Enums;
using Alcohol.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Alcohol.Common;

namespace Alcohol.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class InventoryTransactionController : ControllerBase
{
    private readonly IInventoryTransactionService _inventoryTransactionService;
    private readonly IInventoryTransactionDetailService _inventoryTransactionDetailService;

    public InventoryTransactionController(
        IInventoryTransactionService inventoryTransactionService,
        IInventoryTransactionDetailService inventoryTransactionDetailService)
    {
        _inventoryTransactionService = inventoryTransactionService;
        _inventoryTransactionDetailService = inventoryTransactionDetailService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<IEnumerable<InventoryTransactionResponseDto>>> GetAllTransactions()
    {
        var transactions = await _inventoryTransactionService.GetAllTransactionsAsync();
        return Ok(new ApiResponse<IEnumerable<InventoryTransactionResponseDto>>(transactions));
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<InventoryTransactionResponseDto>> GetTransactionById(int id)
    {
        var transaction = await _inventoryTransactionService.GetTransactionByIdAsync(id);
        if (transaction == null)
            return NotFound(new ApiResponse<string>("Transaction not found"));

        return Ok(new ApiResponse<InventoryTransactionResponseDto>(transaction));
    }

    [HttpGet("product/{productId}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<IEnumerable<InventoryTransactionResponseDto>>> GetTransactionsByProduct(int productId)
    {
        var transactions = await _inventoryTransactionService.GetTransactionsByProductAsync(productId);
        return Ok(new ApiResponse<IEnumerable<InventoryTransactionResponseDto>>(transactions));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<InventoryTransactionResponseDto>> CreateTransaction(InventoryTransactionCreateDto createDto)
    {
        var transaction = await _inventoryTransactionService.CreateTransactionAsync(createDto);
        return CreatedAtAction(nameof(GetTransactionById), 
            new { id = transaction.Id }, 
            new ApiResponse<InventoryTransactionResponseDto>(transaction));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<InventoryTransactionResponseDto>> UpdateTransaction(int id, InventoryTransactionUpdateDto updateDto)
    {
        var transaction = await _inventoryTransactionService.UpdateTransactionAsync(id, updateDto);
        if (transaction == null)
            return NotFound(new ApiResponse<string>("Transaction not found"));

        return Ok(new ApiResponse<InventoryTransactionResponseDto>(transaction));
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult> UpdateTransactionStatus(int id, InventoryTransactionStatusType status)
    {
        var result = await _inventoryTransactionService.UpdateTransactionStatusAsync(id, status);
        if (!result)
            return NotFound(new ApiResponse<string>("Transaction not found"));

        return Ok(new ApiResponse<string>("Transaction status updated successfully"));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteTransaction(int id)
    {
        var result = await _inventoryTransactionService.DeleteTransactionAsync(id);
        if (!result)
            return NotFound(new ApiResponse<string>("Transaction not found"));

        return Ok(new ApiResponse<string>("Transaction deleted successfully"));
    }

    [HttpGet("{transactionId}/details")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<IEnumerable<InventoryTransactionDetailResponseDto>>> GetDetailsByTransaction(int transactionId)
    {
        var details = await _inventoryTransactionDetailService.GetDetailsByTransactionAsync(transactionId);
        return Ok(new ApiResponse<IEnumerable<InventoryTransactionDetailResponseDto>>(details));
    }

    [HttpGet("{transactionId}/details/{detailId}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<InventoryTransactionDetailResponseDto>> GetDetailById(int transactionId, int detailId)
    {
        var detail = await _inventoryTransactionDetailService.GetDetailByIdAsync(detailId);
        if (detail == null)
            return NotFound(new ApiResponse<string>("Transaction detail not found"));

        return Ok(new ApiResponse<InventoryTransactionDetailResponseDto>(detail));
    }

    [HttpPost("{transactionId}/details")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<InventoryTransactionDetailResponseDto>> CreateDetail(int transactionId, InventoryTransactionDetailCreateDto createDto)
    {
        var detail = await _inventoryTransactionDetailService.CreateDetailAsync(createDto);
        return CreatedAtAction(nameof(GetDetailById), 
            new { transactionId = transactionId, detailId = detail.Id }, 
            new ApiResponse<InventoryTransactionDetailResponseDto>(detail));
    }

    [HttpPut("{transactionId}/details/{detailId}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<InventoryTransactionDetailResponseDto>> UpdateDetail(int transactionId, int detailId, InventoryTransactionDetailUpdateDto updateDto)
    {
        var detail = await _inventoryTransactionDetailService.UpdateDetailAsync(detailId, updateDto);
        if (detail == null)
            return NotFound(new ApiResponse<string>("Transaction detail not found"));

        return Ok(new ApiResponse<InventoryTransactionDetailResponseDto>(detail));
    }

    [HttpDelete("{transactionId}/details/{detailId}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteDetail(int transactionId, int detailId)
    {
        var result = await _inventoryTransactionDetailService.DeleteDetailAsync(detailId);
        if (!result)
            return NotFound(new ApiResponse<string>("Transaction detail not found"));

        return Ok(new ApiResponse<string>("Transaction detail deleted successfully"));
    }
} 