using System.ComponentModel.DataAnnotations;

namespace Alcohol.DTOs.Inventory;

/// <summary>
/// Request DTO for adjusting stock with reason and notes
/// </summary>
public class AdjustStockRequest
{
    [Required(ErrorMessage = "Quantity is required")]
    public int Quantity { get; set; }
    
    [Required(ErrorMessage = "Reason is required")]
    public string Reason { get; set; }
    
    public string Notes { get; set; }
}
