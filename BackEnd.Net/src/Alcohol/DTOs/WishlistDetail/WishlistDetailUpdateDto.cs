using System.ComponentModel.DataAnnotations;

namespace Alcohol.DTOs.WishlistDetail;

public class WishlistDetailUpdateDto
{
    [Required]
    public int ProductId { get; set; }
} 