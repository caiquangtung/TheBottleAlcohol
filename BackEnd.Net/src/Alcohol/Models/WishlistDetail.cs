using System;

namespace Alcohol.Models;

public class WishlistDetail
{
    public int Id { get; set; }
    public int WishlistId { get; set; }
    public int ProductId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual Wishlist Wishlist { get; set; }
    public virtual Product Product { get; set; }
} 