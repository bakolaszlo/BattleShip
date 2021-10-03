using Microsoft.EntityFrameworkCore.Migrations;

namespace BattleShip.Migrations
{
    public partial class MatchMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LobbyId",
                table: "Lobby");

            migrationBuilder.AddColumn<int>(
                name: "LobbyId",
                table: "Match",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LobbyId",
                table: "Match");

            migrationBuilder.AddColumn<int>(
                name: "LobbyId",
                table: "Lobby",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
