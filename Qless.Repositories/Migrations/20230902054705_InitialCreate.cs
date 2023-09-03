using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Qless.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CardTypes",
                columns: table => new
                {
                    CardTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BaseRate = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    StartingBalance = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    MaximumBalance = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    MaximumIdleDurationYears = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardTypes", x => x.CardTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Discounts",
                columns: table => new
                {
                    DiscountId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DiscountValue = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    IsFlatDiscount = table.Column<bool>(type: "bit", nullable: false),
                    AvailmentLimit = table.Column<int>(type: "int", nullable: false),
                    AvailmentLimitPerDay = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discounts", x => x.DiscountId);
                });

            migrationBuilder.CreateTable(
                name: "Cards",
                columns: table => new
                {
                    CardId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CardTypeId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresByDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUsed = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CurrentBalance = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    InTransit = table.Column<bool>(type: "bit", nullable: false),
                    TripCount = table.Column<int>(type: "int", nullable: false),
                    TripCountToday = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cards", x => x.CardId);
                    table.ForeignKey(
                        name: "FK_Cards_CardTypes_CardTypeId",
                        column: x => x.CardTypeId,
                        principalTable: "CardTypes",
                        principalColumn: "CardTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CardTypeDiscount",
                columns: table => new
                {
                    CardTypeId = table.Column<int>(type: "int", nullable: false),
                    DiscountId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardTypeDiscount", x => new { x.CardTypeId, x.DiscountId });
                    table.ForeignKey(
                        name: "FK_CardTypeDiscount_CardTypes_CardTypeId",
                        column: x => x.CardTypeId,
                        principalTable: "CardTypes",
                        principalColumn: "CardTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CardTypeDiscount_Discounts_DiscountId",
                        column: x => x.DiscountId,
                        principalTable: "Discounts",
                        principalColumn: "DiscountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "CardTypes",
                columns: new[] { "CardTypeId", "BaseRate", "DateCreated", "DateModified", "Description", "MaximumBalance", "MaximumIdleDurationYears", "Name", "StartingBalance" },
                values: new object[,]
                {
                    { 1, 15m, new DateTime(2023, 9, 2, 5, 47, 5, 632, DateTimeKind.Utc).AddTicks(1086), null, "regular card", 10000m, 5, "regular", 100m },
                    { 2, 10m, new DateTime(2023, 9, 2, 5, 47, 5, 632, DateTimeKind.Utc).AddTicks(1092), null, "discounted card", 10000m, 3, "discounted", 500m },
                    { 3, 10m, new DateTime(2023, 9, 2, 5, 47, 5, 632, DateTimeKind.Utc).AddTicks(1094), null, "special discounted card", 10000m, 3, "specialDiscounted", 500m }
                });

            migrationBuilder.InsertData(
                table: "Discounts",
                columns: new[] { "DiscountId", "AvailmentLimit", "AvailmentLimitPerDay", "DateCreated", "DateModified", "Description", "DiscountValue", "EndDate", "IsFlatDiscount", "Name", "StartDate", "Type" },
                values: new object[,]
                {
                    { 1, -1, -1, new DateTime(2023, 9, 2, 5, 47, 5, 632, DateTimeKind.Utc).AddTicks(1202), null, "Base 20% discount for special cards", 20m, null, false, "base", null, "basic" },
                    { 2, -1, 4, new DateTime(2023, 9, 2, 5, 47, 5, 632, DateTimeKind.Utc).AddTicks(1205), null, "3% discount for succeeding trips", 3m, null, false, "succeeding", null, "special" }
                });

            migrationBuilder.InsertData(
                table: "CardTypeDiscount",
                columns: new[] { "CardTypeId", "DiscountId" },
                values: new object[,]
                {
                    { 3, 1 },
                    { 3, 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cards_CardTypeId",
                table: "Cards",
                column: "CardTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CardTypeDiscount_DiscountId",
                table: "CardTypeDiscount",
                column: "DiscountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cards");

            migrationBuilder.DropTable(
                name: "CardTypeDiscount");

            migrationBuilder.DropTable(
                name: "CardTypes");

            migrationBuilder.DropTable(
                name: "Discounts");
        }
    }
}
