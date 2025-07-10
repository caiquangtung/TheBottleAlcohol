using System;

namespace Alcohol.DTOs.ImportOrder;

public class ImportOrderFilterDto : BaseFilterDto
{
    public int? SupplierId { get; set; }
    public string Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? MinTotal { get; set; }
    public decimal? MaxTotal { get; set; }
} 