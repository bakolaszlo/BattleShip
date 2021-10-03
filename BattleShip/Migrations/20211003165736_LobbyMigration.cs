using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace BattleShip.Migrations
{
    public partial class LobbyMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Lobby",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HostId = table.Column<int>(type: "integer", nullable: true),
                    GuestId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lobby", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lobby_Player_GuestId",
                        column: x => x.GuestId,
                        principalTable: "Player",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Lobby_Player_HostId",
                        column: x => x.HostId,
                        principalTable: "Player",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Lobby_GuestId",
                table: "Lobby",
                column: "GuestId");

            migrationBuilder.CreateIndex(
                name: "IX_Lobby_HostId",
                table: "Lobby",
                column: "HostId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Lobby");
        }
    }
}
