using System;

namespace Alcohol.DTOs.ImportOrder;

public class ImportOrderFilterDto
{
    public string SearchTerm { get; set; }
    public int? SupplierId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
} 