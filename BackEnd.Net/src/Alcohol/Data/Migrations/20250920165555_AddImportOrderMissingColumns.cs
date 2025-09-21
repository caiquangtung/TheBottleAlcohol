using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Alcohol.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddImportOrderMissingColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add OrderDate column to ImportOrder if it doesn't exist
            migrationBuilder.AddColumn<DateTime>(
                name: "OrderDate",
                table: "ImportOrder",
                type: "datetime(6)",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP(6)");

            // Add OrderNumber column to ImportOrder if it doesn't exist
            migrationBuilder.AddColumn<string>(
                name: "OrderNumber",
                table: "ImportOrder",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            // Add Notes column to ImportOrder if it doesn't exist
            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "ImportOrder",
                type: "longtext",
                nullable: true);

            // Update OrderNumber for existing records
            migrationBuilder.Sql(@"
                UPDATE ImportOrder 
                SET OrderNumber = CONCAT('IMP', DATE_FORMAT(CreatedAt, '%Y%m%d%H%i%s'), LPAD(Id, 4, '0'))
                WHERE OrderNumber = '' OR OrderNumber IS NULL;
            ");

            // Update OrderDate for existing records
            migrationBuilder.Sql(@"
                UPDATE ImportOrder 
                SET OrderDate = CreatedAt
                WHERE OrderDate IS NULL OR OrderDate = '1900-01-01';
            ");

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
            // Drop OrderDate column
            migrationBuilder.DropColumn(
                name: "OrderDate",
                table: "ImportOrder");

            // Drop OrderNumber column
            migrationBuilder.DropColumn(
                name: "OrderNumber",
                table: "ImportOrder");

            // Drop Notes column
            migrationBuilder.DropColumn(
                name: "Notes",
                table: "ImportOrder");

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
