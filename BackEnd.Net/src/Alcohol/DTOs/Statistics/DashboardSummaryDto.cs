using System.Collections.Generic;

namespace Alcohol.DTOs.Statistics
{
    public class DashboardSummaryDto
    {
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalProfit { get; set; }
        public decimal AverageOrderValue { get; set; }
        public List<ProductAnalyticsDto> TopProducts { get; set; }
    }
} 