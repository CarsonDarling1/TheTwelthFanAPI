using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheTwelthFanAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddPlayersTableDataFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FantasyLeagueID",
                table: "player",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FantasyTeamID",
                table: "player",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FantasyLeagueID",
                table: "player");

            migrationBuilder.DropColumn(
                name: "FantasyTeamID",
                table: "player");
        }
    }
}
