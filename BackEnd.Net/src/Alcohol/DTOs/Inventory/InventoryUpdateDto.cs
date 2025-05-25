using System.ComponentModel.DataAnnotations;

namespace Alcohol.DTOs.Inventory;

public class InventoryUpdateDto
{
    [Required(ErrorMessage = "Product ID is required")]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "Quantity is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Quantity must be greater than or equal to 0")]
    public int Quantity { get; set; }

    public string Notes { get; set; }
} 