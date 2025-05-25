using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Alcohol.Models
{
    public class Category
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
        public int DisplayOrder { get; set; }
        public string ImageUrl { get; set; }
        
        [Required]
        public bool IsActive { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public int? ParentId { get; set; }

        // Navigation properties
        [ForeignKey("ParentId")]
        public Category Parent { get; set; }
        public ICollection<Category> Children { get; set; }
        public ICollection<Product> Products { get; set; }

        public Category()
        {
            Children = new HashSet<Category>();
            Products = new HashSet<Product>();
        }
    }
} 