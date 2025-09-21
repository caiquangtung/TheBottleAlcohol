using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Alcohol.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryTransactionColumnsManual : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add TransactionNumber column if it doesn't exist
            migrationBuilder.Sql(@"
                ALTER TABLE InventoryTransactions 
                ADD COLUMN IF NOT EXISTS TransactionNumber VARCHAR(50) NOT NULL DEFAULT '';
            ");

            // Add TransactionDate column if it doesn't exist
            migrationBuilder.Sql(@"
                ALTER TABLE InventoryTransactions 
                ADD COLUMN IF NOT EXISTS TransactionDate DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6);
            ");

            // Rename Type column to TransactionType if it exists
            migrationBuilder.Sql(@"
                ALTER TABLE InventoryTransactions 
                CHANGE COLUMN Type TransactionType INT NOT NULL;
            ");

            // Update TransactionNumber for existing records
            migrationBuilder.Sql(@"
                UPDATE InventoryTransactions 
                SET TransactionNumber = CONCAT('TXN', DATE_FORMAT(CreatedAt, '%Y%m%d%H%i%s'), LPAD(Id, 4, '0'))
                WHERE TransactionNumber = '' OR TransactionNumber IS NULL;
            ");

            // Update TransactionDate for existing records
            migrationBuilder.Sql(@"
                UPDATE InventoryTransactions 
                SET TransactionDate = CreatedAt
                WHERE TransactionDate IS NULL OR TransactionDate = '1900-01-01';
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Rename TransactionType back to Type
            migrationBuilder.Sql(@"
                ALTER TABLE InventoryTransactions 
                CHANGE COLUMN TransactionType Type INT NOT NULL;
            ");

            // Drop TransactionNumber column
            migrationBuilder.DropColumn(
                name: "TransactionNumber",
                table: "InventoryTransactions");

            // Drop TransactionDate column
            migrationBuilder.DropColumn(
                name: "TransactionDate",
                table: "InventoryTransactions");
        }
    }
}
