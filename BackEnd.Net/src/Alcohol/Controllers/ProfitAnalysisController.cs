using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.DTOs.Product;
using Alcohol.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Alcohol.Common;

namespace Alcohol.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ProfitAnalysisController : ControllerBase
{
    private readonly ProfitAnalysisService _profitAnalysisService;

    public ProfitAnalysisController(ProfitAnalysisService profitAnalysisService)
    {
        _profitAnalysisService = profitAnalysisService;
    }

    [HttpGet("product/{productId}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetProductProfitAnalysis(int productId)
    {
        try
        {
            var analysis = await _profitAnalysisService.GetProductProfitAnalysisAsync(productId);
            return Ok(new ApiResponse<ProfitAnalysisDto>(analysis));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<string>(ex.Message));
        }
    }

    [HttpGet("all")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetAllProductsProfitAnalysis()
    {
        var analyses = await _profitAnalysisService.GetAllProductsProfitAnalysisAsync();
        return Ok(new ApiResponse<List<ProfitAnalysisDto>>(analyses));
    }

    [HttpGet("summary")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetProfitSummary()
    {
        var summary = await _profitAnalysisService.GetProfitSummaryAsync();
        return Ok(new ApiResponse<ProfitSummaryDto>(summary));
    }

    [HttpGet("low-profit")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetLowProfitProducts([FromQuery] decimal threshold = 15.0m)
    {
        var lowProfitProducts = await _profitAnalysisService.GetLowProfitProductsAsync(threshold);
        return Ok(new ApiResponse<List<ProfitAnalysisDto>>(lowProfitProducts));
    }

    [HttpGet("high-performing")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetHighPerformingProducts()
    {
        var highPerformingProducts = await _profitAnalysisService.GetHighPerformingProductsAsync();
        return Ok(new ApiResponse<List<ProfitAnalysisDto>>(highPerformingProducts));
    }

    [HttpPatch("product/{productId}/target-margin")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> SetTargetMargin(int productId, [FromBody] SetTargetMarginRequest request)
    {
        try
        {
            var analysis = await _profitAnalysisService.SetTargetMarginAsync(productId, request.TargetMarginPercentage);
            return Ok(new ApiResponse<ProfitAnalysisDto>(analysis));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<string>(ex.Message));
        }
    }

    [HttpGet("product/{productId}/recommended-price")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> CalculateRecommendedPrice(int productId, [FromQuery] decimal targetMargin)
    {
        try
        {
            var recommendedPrice = await _profitAnalysisService.CalculateRecommendedPriceAsync(productId, targetMargin);
            return Ok(new ApiResponse<decimal>(recommendedPrice));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<string>(ex.Message));
        }
    }

    [HttpPatch("product/{productId}/update-price-by-margin")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> UpdatePriceByTargetMargin(int productId, [FromBody] CalculatePriceRequest request)
    {
        try
        {
            var analysis = await _profitAnalysisService.UpdatePriceByTargetMarginAsync(productId, request.TargetMarginPercentage);
            return Ok(new ApiResponse<ProfitAnalysisDto>(analysis));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<string>(ex.Message));
        }
    }
}
