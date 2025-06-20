using System;

namespace Alcohol.DTOs.Category
{
    public class CategoryResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Slug { get; set; }
        public int? ParentId { get; set; }
        public string ParentName { get; set; }
        public string ParentSlug { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public string ImageUrl { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public int ProductCount { get; set; }
        public int ChildrenCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
} 