using System;

namespace Alcohol.DTOs.ImportOrder;

public class ImportOrderDetailResponseDto
{
    public int Id { get; set; }
    public int ImportOrderId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
} 