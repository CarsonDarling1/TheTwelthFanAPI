using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheTwelthFanAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddPlayerStats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "interceptions",
                table: "player",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "passingTouchdowns",
                table: "player",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "passingYards",
                table: "player",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "pointLastSeason",
                table: "player",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "receivingTouchdowns",
                table: "player",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "receivingYards",
                table: "player",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "rushingTouchdowns",
                table: "player",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "rushingYards",
                table: "player",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "interceptions",
                table: "player");

            migrationBuilder.DropColumn(
                name: "passingTouchdowns",
                table: "player");

            migrationBuilder.DropColumn(
                name: "passingYards",
                table: "player");

            migrationBuilder.DropColumn(
                name: "pointLastSeason",
                table: "player");

            migrationBuilder.DropColumn(
                name: "receivingTouchdowns",
                table: "player");

            migrationBuilder.DropColumn(
                name: "receivingYards",
                table: "player");

            migrationBuilder.DropColumn(
                name: "rushingTouchdowns",
                table: "player");

            migrationBuilder.DropColumn(
                name: "rushingYards",
                table: "player");
        }
    }
}
