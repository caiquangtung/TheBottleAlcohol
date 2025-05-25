using System;

namespace Alcohol.DTOs.ImportOrderDetail;

public class ImportOrderDetailResponseDto
{
    public int ImportOrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public int ImportPrice { get; set; }
    public int TotalAmount { get; set; }
    public int Status { get; set; }
}
