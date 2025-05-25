using System.ComponentModel.DataAnnotations;

namespace Alcohol.DTOs.Category
{
    public class CategoryUpdateDto
    {
        [Required(ErrorMessage = "Category name is required")]
        [StringLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        public string Slug { get; set; }

        public int? ParentId { get; set; }

        public bool Status { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; }

        public string ImageUrl { get; set; }

        [StringLength(200)]
        public string MetaTitle { get; set; }

        [StringLength(500)]
        public string MetaDescription { get; set; }
    }
} 