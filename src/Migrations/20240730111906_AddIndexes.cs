using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NauraaBot.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Rarity_ID",
                table: "Rarity",
                column: "ID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Faction_ID",
                table: "Faction",
                column: "ID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CardType_ID",
                table: "CardType",
                column: "ID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CardSet_ID",
                table: "CardSet",
                column: "ID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Rarity_ID",
                table: "Rarity");

            migrationBuilder.DropIndex(
                name: "IX_Faction_ID",
                table: "Faction");

            migrationBuilder.DropIndex(
                name: "IX_CardType_ID",
                table: "CardType");

            migrationBuilder.DropIndex(
                name: "IX_CardSet_ID",
                table: "CardSet");
        }
    }
}
