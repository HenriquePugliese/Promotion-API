using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Acropolis.Infrastructure.Contexts.Migrations
{
    public partial class DropIndexesAttributeTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Attributes_AttributeKey",
                table: "Attributes");

            migrationBuilder.DropIndex(
                name: "IX_Attributes_AttributeValue",
                table: "Attributes");

            migrationBuilder.CreateIndex(
                name: "IX_Attributes_AttributeKey",
                table: "Attributes",
                column: "AttributeKey");

            migrationBuilder.CreateIndex(
                name: "IX_Attributes_AttributeValue",
                table: "Attributes",
                column: "AttributeValue");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Attributes_AttributeKey",
                table: "Attributes");

            migrationBuilder.DropIndex(
                name: "IX_Attributes_AttributeValue",
                table: "Attributes");

            migrationBuilder.CreateIndex(
                name: "IX_Attributes_AttributeKey",
                table: "Attributes",
                column: "AttributeKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attributes_AttributeValue",
                table: "Attributes",
                column: "AttributeValue",
                unique: true);
        }
    }
}
