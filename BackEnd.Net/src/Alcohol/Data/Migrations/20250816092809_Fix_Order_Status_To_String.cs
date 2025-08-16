using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Alcohol.Data.Migrations
{
    /// <inheritdoc />
    public partial class Fix_Order_Status_To_String : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Add temporary string column
            migrationBuilder.AddColumn<string>(
                name: "StatusString",
                table: "Order",
                type: "longtext",
                nullable: true,
                collation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4");

            // Step 2: Convert int values to string enum names
            migrationBuilder.Sql(@"
                UPDATE `Order` SET `StatusString` = 
                CASE 
                    WHEN `Status` = 0 THEN 'Pending'
                    WHEN `Status` = 1 THEN 'Paid'
                    WHEN `Status` = 2 THEN 'Processing'
                    WHEN `Status` = 3 THEN 'Shipped'
                    WHEN `Status` = 4 THEN 'Delivered'
                    WHEN `Status` = 5 THEN 'Cancelled'
                    ELSE 'Pending'
                END
            ");

            // Step 3: Drop old Status column
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Order");

            // Step 4: Rename StatusString to Status
            migrationBuilder.RenameColumn(
                name: "StatusString",
                table: "Order",
                newName: "Status");

            // Step 5: Make Status column NOT NULL
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Order",
                type: "longtext",
                nullable: false,
                collation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RowVersion",
                table: "Cart",
                type: "timestamp(6)",
                rowVersion: true,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp(6)",
                oldRowVersion: true,
                oldNullable: true)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Reverse: Step 1: Add temporary int column
            migrationBuilder.AddColumn<int>(
                name: "StatusInt",
                table: "Order",
                type: "int",
                nullable: true);

            // Step 2: Convert string enum names back to int values
            migrationBuilder.Sql(@"
                UPDATE `Order` SET `StatusInt` = 
                CASE 
                    WHEN `Status` = 'Pending' THEN 0
                    WHEN `Status` = 'Paid' THEN 1
                    WHEN `Status` = 'Processing' THEN 2
                    WHEN `Status` = 'Shipped' THEN 3
                    WHEN `Status` = 'Delivered' THEN 4
                    WHEN `Status` = 'Cancelled' THEN 5
                    ELSE 0
                END
            ");

            // Step 3: Drop old Status column
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Order");

            // Step 4: Rename StatusInt to Status
            migrationBuilder.RenameColumn(
                name: "StatusInt",
                table: "Order",
                newName: "Status");

            // Step 5: Make Status column NOT NULL
            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Order",
                type: "int",
                nullable: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "RowVersion",
                table: "Cart",
                type: "timestamp(6)",
                rowVersion: true,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp(6)",
                oldRowVersion: true,
                oldNullable: true)
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);
        }
    }
}
