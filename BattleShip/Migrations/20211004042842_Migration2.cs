using Microsoft.EntityFrameworkCore.Migrations;

namespace BattleShip.Migrations
{
    public partial class Migration2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Match_Player_GuestId",
                table: "Match");

            migrationBuilder.DropForeignKey(
                name: "FK_Match_Player_HostId",
                table: "Match");

            migrationBuilder.DropIndex(
                name: "IX_Match_GuestId",
                table: "Match");

            migrationBuilder.DropIndex(
                name: "IX_Match_HostId",
                table: "Match");

            migrationBuilder.AlterColumn<int>(
                name: "HostId",
                table: "Match",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "GuestId",
                table: "Match",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "HostId",
                table: "Match",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "GuestId",
                table: "Match",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "IX_Match_GuestId",
                table: "Match",
                column: "GuestId");

            migrationBuilder.CreateIndex(
                name: "IX_Match_HostId",
                table: "Match",
                column: "HostId");

            migrationBuilder.AddForeignKey(
                name: "FK_Match_Player_GuestId",
                table: "Match",
                column: "GuestId",
                principalTable: "Player",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Match_Player_HostId",
                table: "Match",
                column: "HostId",
                principalTable: "Player",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
