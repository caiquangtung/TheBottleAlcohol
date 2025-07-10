using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Alcohol.Models;

public class Wishlist
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int CustomerId { get; set; }
    
    [ForeignKey("CustomerId")]
    public Account Customer { get; set; }
    
    [Required]
    public DateTime CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
    
    public ICollection<WishlistDetail> WishlistDetails { get; set; }
    
    public Wishlist()
    {
        WishlistDetails = new List<WishlistDetail>();
        CreatedAt = DateTime.UtcNow;
    }
} 