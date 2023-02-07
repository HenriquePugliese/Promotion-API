using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Acropolis.Infrastructure.Contexts.Migrations
{
    public partial class AddIndexesUniqueCustomers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Customers_Cnpj_SellerId",
                table: "Customers");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Cnpj_SellerId",
                table: "Customers",
                columns: new[] { "Cnpj", "SellerId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Customers_Cnpj_SellerId",
                table: "Customers");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Cnpj_SellerId",
                table: "Customers",
                columns: new[] { "Cnpj", "SellerId" });
        }
    }
}
