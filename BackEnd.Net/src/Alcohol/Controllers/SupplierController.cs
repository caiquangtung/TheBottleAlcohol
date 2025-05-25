using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.DTOs.Supplier;
using Alcohol.DTOs.ImportOrder;
using Alcohol.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Alcohol.Common;

namespace Alcohol.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private readonly ISupplierService _service;

        public SupplierController(ISupplierService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Roles = "CEO,Manager")]
        public async Task<IActionResult> GetAll([FromQuery] SupplierFilterDto filter)
        {
            var result = await _service.GetAllSuppliersAsync();
            return Ok(new ApiResponse<IEnumerable<SupplierResponseDto>>(result));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "CEO,Manager")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var supplier = await _service.GetSupplierByIdAsync(id);
                if (supplier == null)
                    return NotFound(new ApiResponse<string>("Supplier not found"));
                return Ok(new ApiResponse<SupplierResponseDto>(supplier));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>(ex.Message));
            }
        }

        [HttpPost]
        [Authorize(Roles = "CEO,Manager")]
        public async Task<IActionResult> Create(SupplierCreateDto supplierDto)
        {
            try
            {
                var supplier = await _service.CreateSupplierAsync(supplierDto);
                return CreatedAtAction(nameof(GetById), new { id = supplier.Id }, new ApiResponse<SupplierResponseDto>(supplier));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>(ex.Message));
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "CEO,Manager")]
        public async Task<IActionResult> Update(int id, SupplierUpdateDto supplierDto)
        {
            try
            {
                var supplier = await _service.UpdateSupplierAsync(id, supplierDto);
                if (supplier == null)
                    return NotFound(new ApiResponse<string>("Supplier not found"));
                return Ok(new ApiResponse<SupplierResponseDto>(supplier));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>(ex.Message));
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "CEO,Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _service.DeleteSupplierAsync(id);
                if (!result)
                    return NotFound(new ApiResponse<string>("Supplier not found"));
                return Ok(new ApiResponse<string>("Supplier deleted successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>(ex.Message));
            }
        }
    }
} 