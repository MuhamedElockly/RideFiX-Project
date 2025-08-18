using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Presistence.Migrations
{
    /// <inheritdoc />
    public partial class addtranactionhistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CoinTopUp",
                table: "CoinTopUp");

            migrationBuilder.DropColumn(
                name: "RideCoins",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsApplied",
                table: "CoinTopUp");

            migrationBuilder.DropColumn(
                name: "StripePaymentIntentId",
                table: "CoinTopUp");

            migrationBuilder.DropColumn(
                name: "StripeSessionId",
                table: "CoinTopUp");

            migrationBuilder.RenameTable(
                name: "CoinTopUp",
                newName: "CoinTopUps");

            migrationBuilder.AddColumn<int>(
                name: "coinChargeEntityId",
                table: "CoinTopUps",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CoinTopUps",
                table: "CoinTopUps",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "CoinChargeEntities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Coins = table.Column<int>(type: "int", nullable: false),
                    AmountCents = table.Column<long>(type: "bigint", nullable: false),
                    ClientSecret = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentIntentId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoinChargeEntities", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CoinTopUps_coinChargeEntityId",
                table: "CoinTopUps",
                column: "coinChargeEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_CoinTopUps_CoinChargeEntities_coinChargeEntityId",
                table: "CoinTopUps",
                column: "coinChargeEntityId",
                principalTable: "CoinChargeEntities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CoinTopUps_CoinChargeEntities_coinChargeEntityId",
                table: "CoinTopUps");

            migrationBuilder.DropTable(
                name: "CoinChargeEntities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CoinTopUps",
                table: "CoinTopUps");

            migrationBuilder.DropIndex(
                name: "IX_CoinTopUps_coinChargeEntityId",
                table: "CoinTopUps");

            migrationBuilder.DropColumn(
                name: "coinChargeEntityId",
                table: "CoinTopUps");

            migrationBuilder.RenameTable(
                name: "CoinTopUps",
                newName: "CoinTopUp");

            migrationBuilder.AddColumn<int>(
                name: "RideCoins",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsApplied",
                table: "CoinTopUp",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "StripePaymentIntentId",
                table: "CoinTopUp",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StripeSessionId",
                table: "CoinTopUp",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CoinTopUp",
                table: "CoinTopUp",
                column: "Id");
        }
    }
}
