using System;

namespace Alcohol.DTOs.WishlistDetail;

public class WishlistDetailResponseDto
{
    public int Id { get; set; }
    public int WishlistId { get; set; }
    public int ProductId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
} 