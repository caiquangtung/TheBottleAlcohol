using System;
using System.Threading.Tasks;
using Alcohol.DTOs.ImportOrder;
using Alcohol.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Alcohol.Common;
using Alcohol.DTOs;

namespace Alcohol.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ImportOrderController : ControllerBase
{
    private readonly IImportOrderService _importOrderService;

    public ImportOrderController(IImportOrderService importOrderService)
    {
        _importOrderService = importOrderService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetAllImportOrders([FromQuery] ImportOrderFilterDto filter)
    {
        var result = await _importOrderService.GetAllImportOrdersAsync(filter);
        return Ok(new ApiResponse<PagedResult<ImportOrderResponseDto>>(result));
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetImportOrderById(int id)
    {
        var importOrder = await _importOrderService.GetImportOrderByIdAsync(id);
        if (importOrder == null)
            return NotFound(new ApiResponse<string>("Import order not found"));
        return Ok(new ApiResponse<ImportOrderResponseDto>(importOrder));
    }

    [HttpGet("supplier/{supplierId}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetImportOrdersBySupplier(int supplierId)
    {
        var importOrders = await _importOrderService.GetImportOrdersBySupplierAsync(supplierId);
        return Ok(new ApiResponse<ImportOrderResponseDto[]>(importOrders.ToArray()));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> CreateImportOrder(ImportOrderCreateDto createDto)
    {
        try
        {
            var importOrder = await _importOrderService.CreateImportOrderAsync(createDto);
            return CreatedAtAction(nameof(GetImportOrderById), new { id = importOrder.Id }, new ApiResponse<ImportOrderResponseDto>(importOrder));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<string>(ex.Message));
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> UpdateImportOrder(int id, ImportOrderUpdateDto updateDto)
    {
        var importOrder = await _importOrderService.UpdateImportOrderAsync(id, updateDto);
        if (importOrder == null)
            return NotFound(new ApiResponse<string>("Import order not found"));
        return Ok(new ApiResponse<ImportOrderResponseDto>(importOrder));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> DeleteImportOrder(int id)
    {
        var result = await _importOrderService.DeleteImportOrderAsync(id);
        if (!result)
            return NotFound(new ApiResponse<string>("Import order not found"));
        return Ok(new ApiResponse<string>("Import order deleted successfully"));
    }
} 