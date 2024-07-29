using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NauraaBot.Migrations
{
    /// <inheritdoc />
    public partial class CardIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Unique_ID",
                table: "Unique",
                column: "ID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Card_ID",
                table: "Card",
                column: "ID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Unique_ID",
                table: "Unique");

            migrationBuilder.DropIndex(
                name: "IX_Card_ID",
                table: "Card");
        }
    }
}
