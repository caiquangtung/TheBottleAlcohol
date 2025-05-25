using System;

namespace Alcohol.DTOs.Review;

public class ReviewResponseDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int AccountId { get; set; }
    public string AccountName { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
} 