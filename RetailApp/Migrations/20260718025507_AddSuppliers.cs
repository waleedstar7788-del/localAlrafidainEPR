using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RetailApp.Migrations
{
    /// <inheritdoc />
    public partial class AddSuppliers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SupplierNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    SupplierName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    CompanyName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Phone1 = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Phone2 = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    WhatsApp = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Country = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    City = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    GoogleMapsLink = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Website = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    TaxNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CommercialRegistration = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    BankName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    BankAccount = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    IBAN = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    SupplierPhotoPath = table.Column<string>(type: "TEXT", nullable: false),
                    OpeningBalance = table.Column<decimal>(type: "TEXT", nullable: false),
                    CurrentBalance = table.Column<decimal>(type: "TEXT", nullable: false),
                    TotalPurchases = table.Column<decimal>(type: "TEXT", nullable: false),
                    TotalPayments = table.Column<decimal>(type: "TEXT", nullable: false),
                    SupplierRating = table.Column<double>(type: "REAL", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastPurchaseDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastPaymentDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_Phone1",
                table: "Suppliers",
                column: "Phone1",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_SupplierNumber",
                table: "Suppliers",
                column: "SupplierNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Suppliers");
        }
    }
}
