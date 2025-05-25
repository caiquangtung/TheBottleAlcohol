using System.ComponentModel.DataAnnotations;

namespace Alcohol.DTOs.InventoryTransaction;

public class InventoryTransactionCreateDto
{
    [Required(ErrorMessage = "Product ID is required")]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "Quantity is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public int Quantity { get; set; }

    [Required(ErrorMessage = "Type is required")]
    public string Type { get; set; }

    [Required(ErrorMessage = "Reference type is required")]
    public string ReferenceType { get; set; }

    public int? ReferenceId { get; set; }

    public string Notes { get; set; }
} 