using Microsoft.EntityFrameworkCore.Migrations;

namespace BattleShip.Migrations
{
    public partial class Migration3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GuestHp",
                table: "Match",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HostHp",
                table: "Match",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GuestHp",
                table: "Match");

            migrationBuilder.DropColumn(
                name: "HostHp",
                table: "Match");
        }
    }
}
