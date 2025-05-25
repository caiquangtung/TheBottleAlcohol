using Alcohol.DTOs.Statistics;
using Alcohol.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Alcohol.Common;

namespace Alcohol.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = "CEO,Manager")]
    public class DashboardController : ControllerBase
    {
        private readonly IStatisticsService _statisticsService;

        public DashboardController(IStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        [HttpGet("orders")]
        public async Task<IActionResult> GetOrderAnalytics([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            try
            {
                var result = await _statisticsService.GetOrderAnalytics(startDate, endDate);
                return Ok(new ApiResponse<List<OrderAnalyticsDto>>(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>(ex.Message));
            }
        }

        [HttpGet("revenue")]
        public async Task<IActionResult> GetRevenueAnalytics([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            try
            {
                var result = await _statisticsService.GetRevenueAnalytics(startDate, endDate);
                return Ok(new ApiResponse<List<RevenueAnalyticsDto>>(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>(ex.Message));
            }
        }

        [HttpGet("products")]
        public async Task<IActionResult> GetProductAnalytics([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            try
            {
                var result = await _statisticsService.GetProductAnalytics(startDate, endDate);
                return Ok(new ApiResponse<List<ProductAnalyticsDto>>(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>(ex.Message));
            }
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetDashboardSummary([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            try
            {
                var orderAnalytics = await _statisticsService.GetOrderAnalytics(startDate, endDate);
                var revenueAnalytics = await _statisticsService.GetRevenueAnalytics(startDate, endDate);
                var productAnalytics = await _statisticsService.GetProductAnalytics(startDate, endDate);

                var summary = new DashboardSummaryDto
                {
                    TotalOrders = orderAnalytics.Sum(x => x.TotalOrders),
                    TotalRevenue = revenueAnalytics.Sum(x => x.TotalRevenue),
                    TotalProfit = revenueAnalytics.Sum(x => x.TotalProfit),
                    AverageOrderValue = revenueAnalytics.Any() ? revenueAnalytics.Average(x => x.AverageOrderValue) : 0,
                    TopProducts = productAnalytics.OrderByDescending(x => x.TotalRevenue).Take(5).ToList()
                };

                return Ok(new ApiResponse<DashboardSummaryDto>(summary));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>(ex.Message));
            }
        }
    }
} 