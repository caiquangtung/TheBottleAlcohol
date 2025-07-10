using System;
using Alcohol.Models.Enums;

namespace Alcohol.DTOs.Order;

public class OrderFilterDto : BaseFilterDto
{
    public int? CustomerId { get; set; }
    public OrderStatusType? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
} 