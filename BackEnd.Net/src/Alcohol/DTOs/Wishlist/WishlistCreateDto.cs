using System.ComponentModel.DataAnnotations;

namespace Alcohol.DTOs.Wishlist;

public class WishlistCreateDto
{
    [Required]
    public int AccountId { get; set; }
} 