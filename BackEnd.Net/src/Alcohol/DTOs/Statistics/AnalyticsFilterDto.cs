using System;

namespace Alcohol.DTOs.Statistics;

public class AnalyticsFilterDto : BaseFilterDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
} 