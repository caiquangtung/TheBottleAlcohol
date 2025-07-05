using System.ComponentModel.DataAnnotations;

namespace Alcohol.DTOs.Review;

public class ReviewUpdateDto
{
    [Required]
    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
    public int Rating { get; set; }

    [Required]
    [StringLength(1000)]
    public string Comment { get; set; }

    public bool? IsApproved { get; set; }
} 