using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Acropolis.Infrastructure.Contexts.Migrations
{
    public partial class DropIndexesUnique : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Parameters_CustomerId_Code_Value",
                table: "Parameters");

            migrationBuilder.DropIndex(
                name: "IX_Customers_Cnpj_SellerId",
                table: "Customers");

            migrationBuilder.CreateIndex(
                name: "IX_Parameters_CustomerId_Code_Value",
                table: "Parameters",
                columns: new[] { "CustomerId", "Code", "Value" });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Cnpj_SellerId",
                table: "Customers",
                columns: new[] { "Cnpj", "SellerId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Parameters_CustomerId_Code_Value",
                table: "Parameters");

            migrationBuilder.DropIndex(
                name: "IX_Customers_Cnpj_SellerId",
                table: "Customers");

            migrationBuilder.CreateIndex(
                name: "IX_Parameters_CustomerId_Code_Value",
                table: "Parameters",
                columns: new[] { "CustomerId", "Code", "Value" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Cnpj_SellerId",
                table: "Customers",
                columns: new[] { "Cnpj", "SellerId" },
                unique: true);
        }
    }
}
