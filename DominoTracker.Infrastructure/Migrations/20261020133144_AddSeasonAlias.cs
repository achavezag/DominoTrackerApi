using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DominoTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSeasonAlias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Seasons_SessionId",
                table: "Seasons");

            migrationBuilder.AddColumn<int>(
                name: "Alias",
                table: "Seasons",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Backfill: numera cada season existente dentro de su propia sesión,
            // en orden de Id, para que el Alias quede autoincremental por SessionId.
            migrationBuilder.Sql(@"
                ;WITH Numbered AS (
                    SELECT Id, ROW_NUMBER() OVER (PARTITION BY SessionId ORDER BY Id) AS rn
                    FROM Seasons
                )
                UPDATE s
                SET s.Alias = n.rn
                FROM Seasons s
                JOIN Numbered n ON s.Id = n.Id;
            ");

            migrationBuilder.CreateIndex(
                name: "IX_Seasons_SessionId_Alias",
                table: "Seasons",
                columns: new[] { "SessionId", "Alias" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Seasons_SessionId_Alias",
                table: "Seasons");

            migrationBuilder.DropColumn(
                name: "Alias",
                table: "Seasons");

            migrationBuilder.CreateIndex(
                name: "IX_Seasons_SessionId",
                table: "Seasons",
                column: "SessionId");
        }
    }
}
