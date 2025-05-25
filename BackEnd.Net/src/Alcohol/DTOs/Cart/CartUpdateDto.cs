using System.ComponentModel.DataAnnotations;

namespace Alcohol.DTOs.Cart;

public class CartUpdateDto
{
    [Required(ErrorMessage = "Customer ID is required")]
    public int CustomerId { get; set; }
} 