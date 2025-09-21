namespace Alcohol.DTOs.Product;

/// <summary>
/// DTO for product profit analysis
/// </summary>
public class ProfitAnalysisDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal AverageCost { get; set; }
    public decimal SellingPrice { get; set; }
    public decimal ProfitPerUnit { get; set; }
    public decimal ProfitMarginPercentage { get; set; }
    public decimal TargetMarginPercentage { get; set; }
    public decimal RecommendedSellingPrice { get; set; }
    public bool IsMarginAchieved { get; set; }
    public int CurrentStock { get; set; }
    public decimal PotentialTotalProfit { get; set; }
}

/// <summary>
/// Request DTO for setting target profit margin
/// </summary>
public class SetTargetMarginRequest
{
    public decimal TargetMarginPercentage { get; set; }
}

/// <summary>
/// Request DTO for calculating recommended price
/// </summary>
public class CalculatePriceRequest
{
    public decimal TargetMarginPercentage { get; set; }
}

/// <summary>
/// DTO for profit summary report
/// </summary>
public class ProfitSummaryDto
{
    public decimal TotalInventoryValue { get; set; }
    public decimal TotalPotentialRevenue { get; set; }
    public decimal TotalPotentialProfit { get; set; }
    public decimal AverageMarginPercentage { get; set; }
    public int ProductsAboveTargetMargin { get; set; }
    public int ProductsBelowTargetMargin { get; set; }
    public List<ProfitAnalysisDto> TopProfitableProducts { get; set; }
    public List<ProfitAnalysisDto> LowProfitProducts { get; set; }
}
