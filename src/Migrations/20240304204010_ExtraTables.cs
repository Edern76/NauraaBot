using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NauraaBot.Migrations
{
    public partial class ExtraTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Card",
                columns: table => new
                {
                    ID = table.Column<string>(type: "TEXT", nullable: false),
                    SetCode = table.Column<string>(type: "TEXT", nullable: true),
                    RarityCode = table.Column<string>(type: "TEXT", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MainFactionID = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentFactionID = table.Column<string>(type: "TEXT", nullable: true),
                    Names_en = table.Column<string>(type: "TEXT", nullable: true),
                    Names_fr = table.Column<string>(type: "TEXT", nullable: true),
                    Names_de = table.Column<string>(type: "TEXT", nullable: true),
                    Names_es = table.Column<string>(type: "TEXT", nullable: true),
                    Names_it = table.Column<string>(type: "TEXT", nullable: true),
                    Effect_en = table.Column<string>(type: "TEXT", nullable: true),
                    Effect_fr = table.Column<string>(type: "TEXT", nullable: true),
                    Effect_de = table.Column<string>(type: "TEXT", nullable: true),
                    Effect_es = table.Column<string>(type: "TEXT", nullable: true),
                    Effect_it = table.Column<string>(type: "TEXT", nullable: true),
                    Costs_Hand = table.Column<int>(type: "INTEGER", nullable: true),
                    Costs_Reserve = table.Column<int>(type: "INTEGER", nullable: true),
                    Power_Forest = table.Column<int>(type: "INTEGER", nullable: true),
                    Power_Mountain = table.Column<int>(type: "INTEGER", nullable: true),
                    Power_Ocean = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Card", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Card_Faction_CurrentFactionID",
                        column: x => x.CurrentFactionID,
                        principalTable: "Faction",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Card_Faction_MainFactionID",
                        column: x => x.MainFactionID,
                        principalTable: "Faction",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "CardSet",
                columns: table => new
                {
                    ID = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardSet", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Card_CurrentFactionID",
                table: "Card",
                column: "CurrentFactionID");

            migrationBuilder.CreateIndex(
                name: "IX_Card_MainFactionID",
                table: "Card",
                column: "MainFactionID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Card");

            migrationBuilder.DropTable(
                name: "CardSet");
        }
    }
}
