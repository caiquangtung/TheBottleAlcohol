using System.ComponentModel.DataAnnotations;

namespace Alcohol.DTOs.Shipping;

public class ShippingCreateDto
{
    [Required]
    public int OrderId { get; set; }

    [Required]
    public int AccountId { get; set; }

    [Required]
    [StringLength(200)]
    public string ShippingAddress { get; set; }

    [Required]
    [StringLength(20)]
    [Phone]
    public string ShippingPhone { get; set; }

    [Required]
    [StringLength(100)]
    public string ShippingName { get; set; }

    public string TrackingNumber { get; set; }
} 