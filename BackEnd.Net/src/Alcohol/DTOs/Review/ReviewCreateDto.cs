using System.ComponentModel.DataAnnotations;

namespace Alcohol.DTOs.Review;

public class ReviewCreateDto
{
    [Required]
    public int ProductId { get; set; }

    [Required]
    public int AccountId { get; set; }

    [Required]
    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
    public int Rating { get; set; }

    [Required]
    [StringLength(1000)]
    public string Comment { get; set; }
} 