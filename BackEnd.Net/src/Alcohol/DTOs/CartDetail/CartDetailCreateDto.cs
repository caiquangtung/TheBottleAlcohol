using System.ComponentModel.DataAnnotations;

namespace Alcohol.DTOs.CartDetail;

public class CartDetailCreateDto
{
    [Required(ErrorMessage = "Cart ID is required")]
    public int CartId { get; set; }

    [Required(ErrorMessage = "Product ID is required")]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "Quantity is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public int Quantity { get; set; }
} 