namespace Alcohol.DTOs.DataMigration;

/// <summary>
/// Report DTO for data migration status
/// </summary>
public class DataMigrationReport
{
    public int TotalProducts { get; set; }
    public int ProductsWithInventory { get; set; }
    public int ProductsWithoutInventory { get; set; }
    public int TotalInventories { get; set; }
    public int InventoriesWithZeroAverageCost { get; set; }
    public int InventoriesWithMismatchedQuantity { get; set; }
    public decimal TotalInventoryValue { get; set; }
}
