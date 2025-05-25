using System.ComponentModel.DataAnnotations;

namespace Alcohol.DTOs.Cart;

public class CartCreateDto
{
    [Required(ErrorMessage = "Customer ID is required")]
    public int CustomerId { get; set; }
} 