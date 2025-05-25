using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Alcohol.Models.Enums;

namespace Alcohol.Models;

public class OrderStatus
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int OrderId { get; set; }

    [ForeignKey("OrderId")]
    public Order Order { get; set; }

    [Required]
    public OrderStatusType Status { get; set; }

    [Required]
    public DateTime UpdateDate { get; set; } = DateTime.UtcNow;

    public OrderStatus()
    {
        Status = OrderStatusType.Pending; // Default value
    }
}