using System;
using Alcohol.Models.Enums;

namespace Alcohol.Models;

public class Shipping
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int AccountId { get; set; }
    public string ShippingAddress { get; set; }
    public string ShippingPhone { get; set; }
    public string ShippingName { get; set; }
    public string TrackingNumber { get; set; }
    public ShippingStatusType Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual Order Order { get; set; }
    public virtual Account Account { get; set; }
} 