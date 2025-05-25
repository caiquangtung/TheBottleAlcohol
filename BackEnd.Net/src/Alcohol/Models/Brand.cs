using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Alcohol.Models
{
    public class Brand
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [StringLength(200)]
        public string Slug { get; set; }

        public string LogoUrl { get; set; }

        public string Website { get; set; }

        public bool IsActive { get; set; } = true;

        public int DisplayOrder { get; set; }

        public string MetaTitle { get; set; }

        public string MetaDescription { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation property for products
        public ICollection<Product> Products { get; set; }
    }
} 