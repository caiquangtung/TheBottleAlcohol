using System;
using System.ComponentModel.DataAnnotations;
using Alcohol.Models.Enums;

namespace Alcohol.DTOs.Order;

public class OrderUpdateDto
{
    [Required(ErrorMessage = "Shipping address is required")]
    public string ShippingAddress { get; set; }

    public string Notes { get; set; }
    public OrderStatusType Status { get; set; }
} 