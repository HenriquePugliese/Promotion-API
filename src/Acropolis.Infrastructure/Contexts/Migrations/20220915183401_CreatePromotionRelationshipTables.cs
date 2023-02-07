using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Acropolis.Infrastructure.Contexts.Migrations
{
    public partial class CreatePromotionRelationshipTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DtEnd",
                table: "Promotions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DtStart",
                table: "Promotions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalId",
                table: "Promotions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UnitMeasurement",
                table: "Promotions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PromotionsAttributes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttributesId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<int>(type: "int", nullable: false),
                    PromotionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotionsAttributes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromotionsAttributes_Promotions_PromotionId",
                        column: x => x.PromotionId,
                        principalTable: "Promotions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PromotionsParameters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PromotionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotionsParameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromotionsParameters_Promotions_PromotionId",
                        column: x => x.PromotionId,
                        principalTable: "Promotions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PromotionsRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TotalAttributes = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(24,2)", nullable: false),
                    Percentage = table.Column<decimal>(type: "decimal(24,2)", nullable: false),
                    PromotionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotionsRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromotionsRules_Promotions_PromotionId",
                        column: x => x.PromotionId,
                        principalTable: "Promotions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PromotionsAttributesSKUs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PromotionAttributeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotionsAttributesSKUs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromotionsAttributesSKUs_PromotionsAttributes_PromotionAttributeId",
                        column: x => x.PromotionAttributeId,
                        principalTable: "PromotionsAttributes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PromotionsAttributes_PromotionId",
                table: "PromotionsAttributes",
                column: "PromotionId");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionsAttributesSKUs_PromotionAttributeId",
                table: "PromotionsAttributesSKUs",
                column: "PromotionAttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionsParameters_PromotionId",
                table: "PromotionsParameters",
                column: "PromotionId");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionsRules_PromotionId",
                table: "PromotionsRules",
                column: "PromotionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PromotionsAttributesSKUs");

            migrationBuilder.DropTable(
                name: "PromotionsParameters");

            migrationBuilder.DropTable(
                name: "PromotionsRules");

            migrationBuilder.DropTable(
                name: "PromotionsAttributes");

            migrationBuilder.DropColumn(
                name: "DtEnd",
                table: "Promotions");

            migrationBuilder.DropColumn(
                name: "DtStart",
                table: "Promotions");

            migrationBuilder.DropColumn(
                name: "ExternalId",
                table: "Promotions");

            migrationBuilder.DropColumn(
                name: "UnitMeasurement",
                table: "Promotions");
        }
    }
}
