using System;
using Alcohol.Models.Enums;

namespace Alcohol.DTOs.Order;

public class OrderFilterDto
{
    public string SearchTerm { get; set; }
    public int? CustomerId { get; set; }
    public OrderStatusType? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
} 