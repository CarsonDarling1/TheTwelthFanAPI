using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheTwelthFanAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdtoPlayers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "userId",
                table: "player",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "userId",
                table: "player");
        }
    }
}
