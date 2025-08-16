using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Alcohol.Data.Migrations
{
    /// <inheritdoc />
    public partial class ManualFix_AddOrderNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add OrderNumber column (for environments where schema lags behind)
            // Note: IF NOT EXISTS is not supported on some MySQL versions
            migrationBuilder.Sql(@"ALTER TABLE `Order` ADD COLUMN `OrderNumber` varchar(50) NOT NULL DEFAULT '';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop OrderNumber column
            migrationBuilder.Sql(@"ALTER TABLE `Order` DROP COLUMN `OrderNumber`;");
        }
    }
}
