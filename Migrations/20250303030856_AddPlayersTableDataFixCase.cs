using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheTwelthFanAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddPlayersTableDataFixCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Team",
                table: "player",
                newName: "team");

            migrationBuilder.RenameColumn(
                name: "Position",
                table: "player",
                newName: "position");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "player",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "JerseyNumber",
                table: "player",
                newName: "jerseynumber");

            migrationBuilder.RenameColumn(
                name: "FantasyTeamID",
                table: "player",
                newName: "fantasyteamid");

            migrationBuilder.RenameColumn(
                name: "FantasyLeagueID",
                table: "player",
                newName: "fantasyleagueid");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "player",
                newName: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "team",
                table: "player",
                newName: "Team");

            migrationBuilder.RenameColumn(
                name: "position",
                table: "player",
                newName: "Position");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "player",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "jerseynumber",
                table: "player",
                newName: "JerseyNumber");

            migrationBuilder.RenameColumn(
                name: "fantasyteamid",
                table: "player",
                newName: "FantasyTeamID");

            migrationBuilder.RenameColumn(
                name: "fantasyleagueid",
                table: "player",
                newName: "FantasyLeagueID");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "player",
                newName: "Id");
        }
    }
}
