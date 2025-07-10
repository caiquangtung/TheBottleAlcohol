using System;

namespace Alcohol.DTOs;

public abstract class BaseFilterDto
{
    public string SearchTerm { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string SortBy { get; set; }
    public string SortOrder { get; set; } = "asc";
} 