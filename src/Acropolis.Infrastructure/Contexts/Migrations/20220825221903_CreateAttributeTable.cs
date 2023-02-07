using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Acropolis.Infrastructure.Contexts.Migrations
{
    public partial class CreateAttributeTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Attributes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttributeKeyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttributeKeyDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AttributeKeyIsBeginOpen = table.Column<bool>(type: "bit", nullable: false),
                    AttributeKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AttributeKeyLabel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AttributeKeyStatus = table.Column<int>(type: "int", nullable: false),
                    AttributeKeyType = table.Column<int>(type: "int", nullable: false),
                    AttributeValueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttributeValueLabel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AttributeValueStatus = table.Column<int>(type: "int", nullable: false),
                    AttributeValue = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attributes", x => x.Id);
                });

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Attributes");
        }
    }
}
