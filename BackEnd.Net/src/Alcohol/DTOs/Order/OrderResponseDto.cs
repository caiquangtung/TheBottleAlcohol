using System;
using System.Collections.Generic;
using Alcohol.DTOs.OrderDetail;
using Alcohol.Models.Enums;

namespace Alcohol.DTOs.Order;

public class OrderResponseDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; }
    public string CustomerEmail { get; set; }
    public string CustomerPhone { get; set; }
    public string ShippingAddress { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatusType Status { get; set; }
    public string Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public ICollection<OrderDetailResponseDto> OrderDetails { get; set; }
}
