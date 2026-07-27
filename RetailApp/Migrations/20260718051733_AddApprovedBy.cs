using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RetailApp.Migrations
{
    /// <inheritdoc />
    public partial class AddApprovedBy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApprovedBy",
                table: "IncomeEntries",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ApprovedBy",
                table: "ExpenseEntries",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "IncomeEntries");

            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "ExpenseEntries");
        }
    }
}
