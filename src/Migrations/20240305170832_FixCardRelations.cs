using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NauraaBot.Migrations
{
    public partial class FixCardRelations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SetCode",
                table: "Card",
                newName: "SetID");

            migrationBuilder.RenameColumn(
                name: "RarityCode",
                table: "Card",
                newName: "RarityID");

            migrationBuilder.CreateIndex(
                name: "IX_Card_RarityID",
                table: "Card",
                column: "RarityID");

            migrationBuilder.CreateIndex(
                name: "IX_Card_SetID",
                table: "Card",
                column: "SetID");

            migrationBuilder.AddForeignKey(
                name: "FK_Card_CardSet_SetID",
                table: "Card",
                column: "SetID",
                principalTable: "CardSet",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Card_Rarity_RarityID",
                table: "Card",
                column: "RarityID",
                principalTable: "Rarity",
                principalColumn: "ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Card_CardSet_SetID",
                table: "Card");

            migrationBuilder.DropForeignKey(
                name: "FK_Card_Rarity_RarityID",
                table: "Card");

            migrationBuilder.DropIndex(
                name: "IX_Card_RarityID",
                table: "Card");

            migrationBuilder.DropIndex(
                name: "IX_Card_SetID",
                table: "Card");

            migrationBuilder.RenameColumn(
                name: "SetID",
                table: "Card",
                newName: "SetCode");

            migrationBuilder.RenameColumn(
                name: "RarityID",
                table: "Card",
                newName: "RarityCode");
        }
    }
}
