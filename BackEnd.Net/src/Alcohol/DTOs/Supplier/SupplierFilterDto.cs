using System;

namespace Alcohol.DTOs.Supplier;

public class SupplierFilterDto
{
    public string SearchTerm { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
} 