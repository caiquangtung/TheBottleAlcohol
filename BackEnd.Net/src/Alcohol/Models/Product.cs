using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Alcohol.Models;

public class Product
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [StringLength(500)]
    public string Description { get; set; }

    [Required]
    [StringLength(100)]
    public string Slug { get; set; }

    [Required]
    [StringLength(100)]
    public string Origin { get; set; }

    [Required]
    [Column(TypeName = "decimal(4,2)")]
    public decimal Volume { get; set; }

    [Required]
    [Column(TypeName = "decimal(4,1)")]
    public decimal AlcoholContent { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    [Required]
    public int StockQuantity { get; set; }

    [Required]
    public bool Status { get; set; }

    [StringLength(500)]
    public string ImageUrl { get; set; }

    // New fields
    public int? Age { get; set; }  // For whiskey age

    [StringLength(100)]
    public string Flavor { get; set; }  // For rum flavor profile

    public int SalesCount { get; set; } = 0;  // Track product popularity

    [StringLength(200)]
    public string MetaTitle { get; set; }

    [StringLength(500)]
    public string MetaDescription { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public int? CategoryId { get; set; }
    public Category Category { get; set; }

    public int? BrandId { get; set; }
    public Brand Brand { get; set; }

    public ICollection<OrderDetail> OrderDetails { get; set; }
    public ICollection<ImportOrderDetail> ImportOrderDetails { get; set; }
    public ICollection<CartDetail> CartDetails { get; set; }
    public ICollection<WishlistDetail> WishlistDetails { get; set; }
    public ICollection<Review> Reviews { get; set; }
    public ICollection<RecipeIngredient> RecipeIngredients { get; set; }

    public virtual Inventory Inventory { get; set; }

    public Product()
    {
        StockQuantity = 0;
        SalesCount = 0;
        CreatedAt = DateTime.UtcNow;
        OrderDetails = new HashSet<OrderDetail>();
        ImportOrderDetails = new HashSet<ImportOrderDetail>();
        CartDetails = new HashSet<CartDetail>();
        WishlistDetails = new HashSet<WishlistDetail>();
        Reviews = new HashSet<Review>();
        RecipeIngredients = new HashSet<RecipeIngredient>();
    }
}
