using System.ComponentModel.DataAnnotations;

namespace Alcohol.DTOs.Wishlist;

public class WishlistUpdateDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; }
} 