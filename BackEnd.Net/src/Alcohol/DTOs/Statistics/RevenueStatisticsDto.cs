using System;

namespace Alcohol.DTOs.Statistics;

public class RevenueStatisticsDto
{
    public DateTime Date { get; set; }
    public int TotalRevenue { get; set; }
    public int TotalOrders { get; set; }
    public int TotalProducts { get; set; }
} 