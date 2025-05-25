using System.ComponentModel.DataAnnotations;

namespace Alcohol.DTOs.Wishlist;

public class WishlistCreateDto
{
    [Required]
    public int AccountId { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }
} 