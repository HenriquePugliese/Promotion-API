using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Acropolis.Infrastructure.Contexts.Migrations
{
    public partial class CreatePromotionCnpjTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PromotionsCnpjs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExternalId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Cnpj = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false),
                    PromotionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotionsCnpjs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromotionsCnpjs_Promotions_PromotionId",
                        column: x => x.PromotionId,
                        principalTable: "Promotions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PromotionsCnpjs_ExternalId_Cnpj",
                table: "PromotionsCnpjs",
                columns: new[] { "ExternalId", "Cnpj" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PromotionsCnpjs_PromotionId",
                table: "PromotionsCnpjs",
                column: "PromotionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PromotionsCnpjs");
        }
    }
}
