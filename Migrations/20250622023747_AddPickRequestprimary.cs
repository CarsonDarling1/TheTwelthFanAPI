using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheTwelthFanAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddPickRequestprimary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DraftOrderEntries_Drafts_DraftId",
                table: "DraftOrderEntries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Drafts",
                table: "Drafts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DraftOrderEntries",
                table: "DraftOrderEntries");

            migrationBuilder.RenameTable(
                name: "Drafts",
                newName: "draft");

            migrationBuilder.RenameTable(
                name: "DraftOrderEntries",
                newName: "draftorderentry");

            migrationBuilder.RenameIndex(
                name: "IX_DraftOrderEntries_DraftId",
                table: "draftorderentry",
                newName: "IX_draftorderentry_DraftId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_draft",
                table: "draft",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_draftorderentry",
                table: "draftorderentry",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "draftpickrequest",
                columns: table => new
                {
                    LeagueId = table.Column<int>(type: "integer", nullable: false),
                    TeamId = table.Column<int>(type: "integer", nullable: false),
                    PlayerId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_draftpickrequest", x => new { x.LeagueId, x.TeamId, x.PlayerId });
                });

            migrationBuilder.AddForeignKey(
                name: "FK_draftorderentry_draft_DraftId",
                table: "draftorderentry",
                column: "DraftId",
                principalTable: "draft",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_draftorderentry_draft_DraftId",
                table: "draftorderentry");

            migrationBuilder.DropTable(
                name: "draftpickrequest");

            migrationBuilder.DropPrimaryKey(
                name: "PK_draftorderentry",
                table: "draftorderentry");

            migrationBuilder.DropPrimaryKey(
                name: "PK_draft",
                table: "draft");

            migrationBuilder.RenameTable(
                name: "draftorderentry",
                newName: "DraftOrderEntries");

            migrationBuilder.RenameTable(
                name: "draft",
                newName: "Drafts");

            migrationBuilder.RenameIndex(
                name: "IX_draftorderentry_DraftId",
                table: "DraftOrderEntries",
                newName: "IX_DraftOrderEntries_DraftId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DraftOrderEntries",
                table: "DraftOrderEntries",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Drafts",
                table: "Drafts",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DraftOrderEntries_Drafts_DraftId",
                table: "DraftOrderEntries",
                column: "DraftId",
                principalTable: "Drafts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
