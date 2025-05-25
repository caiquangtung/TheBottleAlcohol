using System;
using System.Collections.Generic;
using Alcohol.Models.Enums;

namespace Alcohol.DTOs.ImportOrder;

public class ImportOrderResponseDto
{
    public int Id { get; set; }
    public int SupplierId { get; set; }
    public string SupplierName { get; set; }
    public int ManagerId { get; set; }
    public string ManagerName { get; set; }
    public decimal TotalAmount { get; set; }
    public ImportOrderStatusType Status { get; set; }
    public string Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public ICollection<ImportOrderDetailResponseDto> ImportOrderDetails { get; set; }
} 