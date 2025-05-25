using System;

namespace Alcohol.DTOs.Wishlist;

public class WishlistResponseDto
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
} 