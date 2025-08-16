using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Alcohol.DTOs.OrderDetail;

namespace Alcohol.DTOs.Order;

public class OrderCreateDto
{
    [Required(ErrorMessage = "Customer ID is required")]
    public int CustomerId { get; set; }

    [Required(ErrorMessage = "Payment method is required")]
    public string PaymentMethod { get; set; }

    [Required(ErrorMessage = "Shipping method is required")]
    public string ShippingMethod { get; set; }

    [Required(ErrorMessage = "Shipping address is required")]
    public string ShippingAddress { get; set; }

    public string Note { get; set; }

    [Required(ErrorMessage = "Order details are required")]
    public List<OrderDetailCreateDto> OrderDetails { get; set; }
} 