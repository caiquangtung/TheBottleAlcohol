using System;

namespace Alcohol.DTOs.Wishlist;

public class WishlistFilterDto : BaseFilterDto
{
    public int? CustomerId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
} 