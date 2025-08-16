using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Alcohol.Data.Migrations
{
    /// <summary>
    /// Manual fix: add PaymentDate column to Payment table if the database schema is missing it.
    /// </summary>
    public partial class ManualFix_AddPaymentDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add PaymentDate column (non-nullable) with default current timestamp for existing rows
            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentDate",
                table: "Payment",
                type: "datetime(6)",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP(6)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentDate",
                table: "Payment");
        }
    }
}


