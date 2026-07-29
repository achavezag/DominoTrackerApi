using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DominoTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWars : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Wars",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TeamAId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeamBId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wars", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wars_Teams_TeamAId",
                        column: x => x.TeamAId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Wars_Teams_TeamBId",
                        column: x => x.TeamBId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WarMatches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WarId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarMatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WarMatches_Matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WarMatches_Wars_WarId",
                        column: x => x.WarId,
                        principalTable: "Wars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WarMatches_MatchId",
                table: "WarMatches",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_WarMatches_WarId_Order",
                table: "WarMatches",
                columns: new[] { "WarId", "Order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wars_TeamAId",
                table: "Wars",
                column: "TeamAId");

            migrationBuilder.CreateIndex(
                name: "IX_Wars_TeamBId",
                table: "Wars",
                column: "TeamBId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WarMatches");

            migrationBuilder.DropTable(
                name: "Wars");
        }
    }
}
