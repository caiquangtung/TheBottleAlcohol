using System.ComponentModel.DataAnnotations;

namespace Alcohol.DTOs.WishlistDetail;

public class WishlistDetailCreateDto
{
    [Required]
    public int WishlistId { get; set; }

    [Required]
    public int ProductId { get; set; }
} 