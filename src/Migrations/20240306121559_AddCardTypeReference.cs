using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NauraaBot.Migrations
{
    public partial class AddCardTypeReference : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TypeID",
                table: "Card",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Card_TypeID",
                table: "Card",
                column: "TypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Card_CardType_TypeID",
                table: "Card",
                column: "TypeID",
                principalTable: "CardType",
                principalColumn: "ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Card_CardType_TypeID",
                table: "Card");

            migrationBuilder.DropIndex(
                name: "IX_Card_TypeID",
                table: "Card");

            migrationBuilder.DropColumn(
                name: "TypeID",
                table: "Card");
        }
    }
}
