using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Alcohol.Models.Enums;

namespace Alcohol.Models;

public class Order
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public PaymentMethodType PaymentMethod { get; set; }
    public ShippingMethodType ShippingMethod { get; set; }
    public string ShippingAddress { get; set; }
    public string ShippingPhone { get; set; }
    public string ShippingName { get; set; }
    public string Note { get; set; }
    public OrderStatusType Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual Account Account { get; set; }
    public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    public virtual ICollection<OrderStatus> OrderStatuses { get; set; }

    public Order()
    {
        OrderDate = DateTime.Now;
        Status = OrderStatusType.Pending;
        CreatedAt = DateTime.Now;
    }
}
