using System;

namespace Alcohol.DTOs.WishlistDetail;

public class WishlistDetailFilterDto : BaseFilterDto
{
    public int? WishlistId { get; set; }
    public int? ProductId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
} 