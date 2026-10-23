using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DominoTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSessions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Teams_Player1Id_Player2Id",
                table: "Teams");

            migrationBuilder.AddColumn<int>(
                name: "SessionId",
                table: "Wars",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "SessionId",
                table: "Teams",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "SessionId",
                table: "Seasons",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "SessionId",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "SessionId",
                table: "Matches",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Seasons",
                keyColumn: "Id",
                keyValue: 1,
                column: "SessionId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Seasons",
                keyColumn: "Id",
                keyValue: 2,
                column: "SessionId",
                value: 1);

            migrationBuilder.InsertData(
                table: "Sessions",
                columns: new[] { "Id", "CreatedAt", "Name" },
                values: new object[] { 1, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sesión principal" });

            migrationBuilder.CreateIndex(
                name: "IX_Wars_SessionId",
                table: "Wars",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_Player1Id",
                table: "Teams",
                column: "Player1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_SessionId_Player1Id_Player2Id",
                table: "Teams",
                columns: new[] { "SessionId", "Player1Id", "Player2Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Seasons_SessionId",
                table: "Seasons",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_SessionId",
                table: "Players",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_SessionId",
                table: "Matches",
                column: "SessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Sessions_SessionId",
                table: "Matches",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Sessions_SessionId",
                table: "Players",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Seasons_Sessions_SessionId",
                table: "Seasons",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_Sessions_SessionId",
                table: "Teams",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Wars_Sessions_SessionId",
                table: "Wars",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Sessions_SessionId",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Players_Sessions_SessionId",
                table: "Players");

            migrationBuilder.DropForeignKey(
                name: "FK_Seasons_Sessions_SessionId",
                table: "Seasons");

            migrationBuilder.DropForeignKey(
                name: "FK_Teams_Sessions_SessionId",
                table: "Teams");

            migrationBuilder.DropForeignKey(
                name: "FK_Wars_Sessions_SessionId",
                table: "Wars");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_Wars_SessionId",
                table: "Wars");

            migrationBuilder.DropIndex(
                name: "IX_Teams_Player1Id",
                table: "Teams");

            migrationBuilder.DropIndex(
                name: "IX_Teams_SessionId_Player1Id_Player2Id",
                table: "Teams");

            migrationBuilder.DropIndex(
                name: "IX_Seasons_SessionId",
                table: "Seasons");

            migrationBuilder.DropIndex(
                name: "IX_Players_SessionId",
                table: "Players");

            migrationBuilder.DropIndex(
                name: "IX_Matches_SessionId",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "Wars");

            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "Seasons");

            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "Matches");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_Player1Id_Player2Id",
                table: "Teams",
                columns: new[] { "Player1Id", "Player2Id" },
                unique: true);
        }
    }
}
