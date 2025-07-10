using System;

namespace Alcohol.DTOs.Review;

public class ReviewFilterDto : BaseFilterDto
{
    public int? ProductId { get; set; }
    public int? CustomerId { get; set; }
    public int? AccountId { get; set; }
    public int? MinRating { get; set; }
    public int? MaxRating { get; set; }
    public int? Rating { get; set; }
    public bool? IsApproved { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
} 