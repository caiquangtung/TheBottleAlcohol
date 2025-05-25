using System.Collections.Generic;

namespace Alcohol.Common;

public class PagedResult<T>
{
    public List<T> Items { get; set; }
    public int TotalItems { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (TotalItems + PageSize - 1) / PageSize;
} 