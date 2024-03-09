using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NauraaBot.Migrations
{
    public partial class AddDiscardEffect : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DiscardEffect_de",
                table: "Card",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DiscardEffect_en",
                table: "Card",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DiscardEffect_es",
                table: "Card",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DiscardEffect_fr",
                table: "Card",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DiscardEffect_it",
                table: "Card",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscardEffect_de",
                table: "Card");

            migrationBuilder.DropColumn(
                name: "DiscardEffect_en",
                table: "Card");

            migrationBuilder.DropColumn(
                name: "DiscardEffect_es",
                table: "Card");

            migrationBuilder.DropColumn(
                name: "DiscardEffect_fr",
                table: "Card");

            migrationBuilder.DropColumn(
                name: "DiscardEffect_it",
                table: "Card");
        }
    }
}
