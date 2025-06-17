using System;
using System.Threading.Tasks;
using Alcohol.DTOs.Inventory;
using Alcohol.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Alcohol.Common;

namespace Alcohol.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetAll()
        {
            var inventories = await _inventoryService.GetAllInventoriesAsync();
            return Ok(new ApiResponse<IEnumerable<InventoryResponseDto>>(inventories));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetById(int id)
        {
            var inventory = await _inventoryService.GetInventoryByIdAsync(id);
            if (inventory == null)
                return NotFound(new ApiResponse<string>("Inventory not found"));
            return Ok(new ApiResponse<InventoryResponseDto>(inventory));
        }

        [HttpGet("product/{productId}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetByProduct(int productId)
        {
            var inventory = await _inventoryService.GetInventoryByProductAsync(productId);
            if (inventory == null)
                return NotFound(new ApiResponse<string>("Inventory not found"));
            return Ok(new ApiResponse<InventoryResponseDto>(inventory));
        }

        [HttpGet("low-stock/{threshold}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetLowStock(int threshold)
        {
            var inventories = await _inventoryService.GetLowStockInventoriesAsync(threshold);
            return Ok(new ApiResponse<IEnumerable<InventoryResponseDto>>(inventories));
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create(InventoryCreateDto createDto)
        {
            var inventory = await _inventoryService.CreateInventoryAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = inventory.Id }, new ApiResponse<InventoryResponseDto>(inventory));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Update(int id, InventoryUpdateDto updateDto)
        {
            var inventory = await _inventoryService.UpdateInventoryAsync(id, updateDto);
            if (inventory == null)
                return NotFound(new ApiResponse<string>("Inventory not found"));
            return Ok(new ApiResponse<InventoryResponseDto>(inventory));
        }

        [HttpPut("{id}/stock")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateStock(int id, [FromBody] int quantity)
        {
            var result = await _inventoryService.UpdateStockAsync(id, quantity);
            if (!result)
                return NotFound(new ApiResponse<string>("Inventory not found"));
            return Ok(new ApiResponse<string>("Stock updated successfully"));
        }

        [HttpPut("{id}/add-stock")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> AddStock(int id, [FromBody] int quantity)
        {
            var result = await _inventoryService.AddStockAsync(id, quantity);
            if (!result)
                return NotFound(new ApiResponse<string>("Inventory not found"));
            return Ok(new ApiResponse<string>("Stock added successfully"));
        }

        [HttpPut("{id}/remove-stock")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> RemoveStock(int id, [FromBody] int quantity)
        {
            var result = await _inventoryService.RemoveStockAsync(id, quantity);
            if (!result)
                return NotFound(new ApiResponse<string>("Inventory not found"));
            return Ok(new ApiResponse<string>("Stock removed successfully"));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _inventoryService.DeleteInventoryAsync(id);
            if (!result)
                return NotFound(new ApiResponse<string>("Inventory not found"));
            return Ok(new ApiResponse<string>("Inventory deleted successfully"));
        }
    }
} 