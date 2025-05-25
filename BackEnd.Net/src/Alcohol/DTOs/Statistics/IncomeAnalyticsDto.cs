using System;

namespace Alcohol.DTOs.Statistics;

public class IncomeAnalyticsDto
{
    public DateTime Date { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalProfit { get; set; }
} 