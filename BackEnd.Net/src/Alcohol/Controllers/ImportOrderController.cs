using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.DTOs.ImportOrder;
using Alcohol.Models.Enums;
using Alcohol.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Alcohol.Common;

namespace Alcohol.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ImportOrderController : ControllerBase
    {
        private readonly IImportOrderService _importOrderService;

        public ImportOrderController(IImportOrderService importOrderService)
        {
            _importOrderService = importOrderService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<IEnumerable<ImportOrderResponseDto>>> GetAllImportOrders()
        {
            var importOrders = await _importOrderService.GetAllImportOrdersAsync();
            return Ok(importOrders);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<ImportOrderResponseDto>> GetImportOrderById(int id)
        {
            var importOrder = await _importOrderService.GetImportOrderByIdAsync(id);
            if (importOrder == null)
                return NotFound();

            return Ok(importOrder);
        }

        [HttpGet("{id}/with-details")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<ImportOrderResponseDto>> GetImportOrderWithDetails(int id)
        {
            var importOrder = await _importOrderService.GetImportOrderWithDetailsAsync(id);
            if (importOrder == null)
                return NotFound();

            return Ok(importOrder);
        }

        [HttpGet("supplier/{supplierId}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<IEnumerable<ImportOrderResponseDto>>> GetImportOrdersBySupplier(int supplierId)
        {
            var importOrders = await _importOrderService.GetImportOrdersBySupplierAsync(supplierId);
            return Ok(importOrders);
        }

        [HttpGet("status/{status}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<IEnumerable<ImportOrderResponseDto>>> GetImportOrdersByStatus(ImportOrderStatusType status)
        {
            var importOrders = await _importOrderService.GetImportOrdersByStatusAsync(status);
            return Ok(importOrders);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<ImportOrderResponseDto>> CreateImportOrder(ImportOrderCreateDto createDto)
        {
            var importOrder = await _importOrderService.CreateImportOrderAsync(createDto);
            return CreatedAtAction(nameof(GetImportOrderById), new { id = importOrder.Id }, importOrder);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<ImportOrderResponseDto>> UpdateImportOrder(int id, ImportOrderUpdateDto updateDto)
        {
            var importOrder = await _importOrderService.UpdateImportOrderAsync(id, updateDto);
            if (importOrder == null)
                return NotFound();

            return Ok(importOrder);
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<ImportOrderResponseDto>> UpdateImportOrderStatus(int id, ImportOrderStatusType status)
        {
            var importOrder = await _importOrderService.UpdateImportOrderStatusAsync(id, status);
            if (importOrder == null)
                return NotFound();

            return Ok(importOrder);
        }

        [HttpPut("{id}/complete")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<ImportOrderResponseDto>> CompleteImportOrder(int id)
        {
            var importOrder = await _importOrderService.CompleteImportOrderAsync(id);
            if (importOrder == null)
                return NotFound();

            return Ok(importOrder);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteImportOrder(int id)
        {
            var result = await _importOrderService.DeleteImportOrderAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpGet("{id}/details")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<List<ImportOrderDetailResponseDto>>> GetImportOrderDetails(int id)
        {
            var details = await _importOrderService.GetImportOrderDetails(id);
            return Ok(details);
        }

        [HttpPost("{id}/details")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<ImportOrderDetailResponseDto>> AddImportOrderDetail(int id, ImportOrderDetailCreateDto detailDto)
        {
            var detail = await _importOrderService.AddImportOrderDetail(id, detailDto);
            return Ok(detail);
        }

        [HttpDelete("{id}/details/{detailId}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult> RemoveImportOrderDetail(int id, int detailId)
        {
            var result = await _importOrderService.RemoveImportOrderDetail(id, detailId);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
} 