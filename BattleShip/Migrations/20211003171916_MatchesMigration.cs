using Microsoft.EntityFrameworkCore.Migrations;

namespace BattleShip.Migrations
{
    public partial class MatchesMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lobby_Player_GuestId",
                table: "Lobby");

            migrationBuilder.DropForeignKey(
                name: "FK_Lobby_Player_HostId",
                table: "Lobby");

            migrationBuilder.DropIndex(
                name: "IX_Lobby_GuestId",
                table: "Lobby");

            migrationBuilder.DropIndex(
                name: "IX_Lobby_HostId",
                table: "Lobby");

            migrationBuilder.DropColumn(
                name: "GuestId",
                table: "Lobby");

            migrationBuilder.RenameColumn(
                name: "HostId",
                table: "Lobby",
                newName: "Guest");

            migrationBuilder.AddColumn<int>(
                name: "Host",
                table: "Lobby",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Host",
                table: "Lobby");

            migrationBuilder.RenameColumn(
                name: "Guest",
                table: "Lobby",
                newName: "HostId");

            migrationBuilder.AddColumn<int>(
                name: "GuestId",
                table: "Lobby",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lobby_GuestId",
                table: "Lobby",
                column: "GuestId");

            migrationBuilder.CreateIndex(
                name: "IX_Lobby_HostId",
                table: "Lobby",
                column: "HostId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lobby_Player_GuestId",
                table: "Lobby",
                column: "GuestId",
                principalTable: "Player",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Lobby_Player_HostId",
                table: "Lobby",
                column: "HostId",
                principalTable: "Player",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
