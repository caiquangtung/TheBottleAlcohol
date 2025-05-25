using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Alcohol.Models
{
    public class Recipe
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

        [StringLength(500)]
        public string ImageUrl { get; set; }

        [Required]
        public string Instructions { get; set; }

        [Required]
        [StringLength(50)]
        public string Difficulty { get; set; }

        [Required]
        public int PreparationTime { get; set; }

        [Required]
        public int Servings { get; set; }

        [Required]
        public int DisplayOrder { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [StringLength(200)]
        public string MetaTitle { get; set; }

        [StringLength(500)]
        public string MetaDescription { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Changed from many-to-many to direct relationship
        public int? CategoryId { get; set; }
        public Category Category { get; set; }

        // Navigation properties
        public ICollection<RecipeIngredient> Ingredients { get; set; }

        public Recipe()
        {
            CreatedAt = DateTime.UtcNow;
            Ingredients = new HashSet<RecipeIngredient>();
        }
    }

    public class RecipeIngredient
    {
        [Key]
        public int Id { get; set; }

        public int RecipeId { get; set; }
        [ForeignKey("RecipeId")]
        public Recipe Recipe { get; set; }

        public int? ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public decimal Quantity { get; set; }

        [StringLength(50)]
        public string Unit { get; set; }

        public string Notes { get; set; }
    }

    public class RecipeCategory
    {
        [Key]
        public int Id { get; set; }

        public int RecipeId { get; set; }
        [ForeignKey("RecipeId")]
        public Recipe Recipe { get; set; }

        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }
    }
} 