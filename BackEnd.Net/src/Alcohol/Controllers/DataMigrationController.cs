using System;
using System.Threading.Tasks;
using Alcohol.DTOs.DataMigration;
using Alcohol.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Alcohol.Common;

namespace Alcohol.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class DataMigrationController : ControllerBase
{
    private readonly DataMigrationService _dataMigrationService;

    public DataMigrationController(DataMigrationService dataMigrationService)
    {
        _dataMigrationService = dataMigrationService;
    }

    [HttpGet("report")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetMigrationReport()
    {
        try
        {
            var report = await _dataMigrationService.GetMigrationReportAsync();
            return Ok(new ApiResponse<DataMigrationReport>(report));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<string>(ex.Message));
        }
    }

    [HttpPost("recalculate-average-costs")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RecalculateAllAverageCosts()
    {
        try
        {
            await _dataMigrationService.RecalculateAllAverageCostsAsync();
            return Ok(new ApiResponse<string>("Average costs recalculated successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<string>(ex.Message));
        }
    }

    [HttpPost("sync-stock-quantities")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SyncAllProductStockQuantities()
    {
        try
        {
            await _dataMigrationService.SyncAllProductStockQuantitiesAsync();
            return Ok(new ApiResponse<string>("Product stock quantities synchronized successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<string>(ex.Message));
        }
    }

    [HttpPost("create-missing-inventories")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateMissingInventoryRecords()
    {
        try
        {
            await _dataMigrationService.CreateMissingInventoryRecordsAsync();
            return Ok(new ApiResponse<string>("Missing inventory records created successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<string>(ex.Message));
        }
    }

    [HttpPost("run-all")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RunAllMigrations()
    {
        try
        {
            await _dataMigrationService.RunAllMigrationsAsync();
            return Ok(new ApiResponse<string>("All data migrations completed successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<string>(ex.Message));
        }
    }
}
