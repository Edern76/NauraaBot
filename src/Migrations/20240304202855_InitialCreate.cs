using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NauraaBot.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Faction",
                columns: table => new
                {
                    ID = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Faction", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Rarity",
                columns: table => new
                {
                    ID = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Short = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rarity", x => x.ID);
                });

            migrationBuilder.InsertData(
                table: "Faction",
                columns: new[] { "ID", "Name" },
                values: new object[] { "AX", "Axiom" });

            migrationBuilder.InsertData(
                table: "Faction",
                columns: new[] { "ID", "Name" },
                values: new object[] { "BR", "Bravos" });

            migrationBuilder.InsertData(
                table: "Faction",
                columns: new[] { "ID", "Name" },
                values: new object[] { "LY", "Lyra" });

            migrationBuilder.InsertData(
                table: "Faction",
                columns: new[] { "ID", "Name" },
                values: new object[] { "MU", "Muna" });

            migrationBuilder.InsertData(
                table: "Faction",
                columns: new[] { "ID", "Name" },
                values: new object[] { "OR", "Ordis" });

            migrationBuilder.InsertData(
                table: "Faction",
                columns: new[] { "ID", "Name" },
                values: new object[] { "YZ", "Yzmir" });

            migrationBuilder.InsertData(
                table: "Rarity",
                columns: new[] { "ID", "Name", "Short" },
                values: new object[] { "COMMON", "Common", "C" });

            migrationBuilder.InsertData(
                table: "Rarity",
                columns: new[] { "ID", "Name", "Short" },
                values: new object[] { "RARE", "Rare", "R" });

            migrationBuilder.InsertData(
                table: "Rarity",
                columns: new[] { "ID", "Name", "Short" },
                values: new object[] { "UNIQUE", "Unique", "U" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Faction");

            migrationBuilder.DropTable(
                name: "Rarity");
        }
    }
}
