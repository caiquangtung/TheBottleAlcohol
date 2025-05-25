using System;

namespace Alcohol.DTOs.Statistics;

public class ProductAnalyticsDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public int TotalQuantity { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalProfit { get; set; }
    public int CurrentStock { get; set; }
} 