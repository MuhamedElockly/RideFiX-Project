using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Presistence.Migrations
{
    /// <inheritdoc />
    public partial class ReportStateAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           /* migrationBuilder.DropColumn(
                name: "AmountCents",
                table: "CoinTopUps");

            migrationBuilder.DropColumn(
                name: "Coins",
                table: "CoinTopUps");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "CoinTopUps");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "CoinTopUps");

            migrationBuilder.DropColumn(
                name: "ProcessedAt",
                table: "CoinTopUps");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "CoinTopUps");*/

            migrationBuilder.AddColumn<int>(
                name: "reportState",
                table: "reports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            //migrationBuilder.AddColumn<int>(
            //    name: "Coins",
            //    table: "AspNetUsers",
            //    type: "int",
            //    nullable: false,
            //    defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "reportState",
                table: "reports");

            //migrationBuilder.DropColumn(
            //    name: "Coins",
            //    table: "AspNetUsers");

            //migrationBuilder.AddColumn<long>(
            //    name: "AmountCents",
            //    table: "CoinTopUps",
            //    type: "bigint",
            //    nullable: false,
            //    defaultValue: 0L);

            //migrationBuilder.AddColumn<int>(
            //    name: "Coins",
            //    table: "CoinTopUps",
            //    type: "int",
            //    nullable: false,
            //    defaultValue: 0);

            //migrationBuilder.AddColumn<DateTime>(
            //    name: "CreatedAt",
            //    table: "CoinTopUps",
            //    type: "datetime2",
            //    nullable: false,
            //    defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            //migrationBuilder.AddColumn<string>(
            //    name: "Currency",
            //    table: "CoinTopUps",
            //    type: "nvarchar(max)",
            //    nullable: false,
            //    defaultValue: "");

            //migrationBuilder.AddColumn<DateTime>(
            //    name: "ProcessedAt",
            //    table: "CoinTopUps",
            //    type: "datetime2",
            //    nullable: true);

            //migrationBuilder.AddColumn<int>(
            //    name: "Status",
            //    table: "CoinTopUps",
            //    type: "int",
            //    nullable: false,
            //    defaultValue: 0);
        }
    }
}
