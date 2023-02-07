using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Acropolis.Infrastructure.Contexts.Migrations
{
    public partial class ChangedColumnAmountPromotionAttributeTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "PromotionsAttributes");

            migrationBuilder.AddColumn<decimal>(
                name: "AmountWeight",
                table: "PromotionsAttributes",
                type: "decimal(18,4)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountWeight",
                table: "PromotionsAttributes");

            migrationBuilder.AddColumn<int>(
                name: "Amount",
                table: "PromotionsAttributes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
