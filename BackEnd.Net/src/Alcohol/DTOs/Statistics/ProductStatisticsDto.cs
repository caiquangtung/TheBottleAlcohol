using System;

namespace Alcohol.DTOs.Statistics;

public class ProductStatisticsDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public int TotalSold { get; set; }
    public int TotalRevenue { get; set; }
} 