using System.ComponentModel.DataAnnotations;

namespace Alcohol.DTOs.CartDetail;

public class CartDetailUpdateDto
{
    [Required(ErrorMessage = "Quantity is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public int Quantity { get; set; }
} 