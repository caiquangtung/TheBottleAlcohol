namespace Alcohol.DTOs.ImportOrder;

/// <summary>
/// Request DTO for cancelling import order
/// </summary>
public class CancelImportOrderRequest
{
    public string Reason { get; set; }
}
