using System.ComponentModel.DataAnnotations;
using Alcohol.Models.Enums;

namespace Alcohol.DTOs.Shipping;

public class ShippingUpdateDto
{
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

    [Required]
    public ShippingStatusType Status { get; set; }
} 