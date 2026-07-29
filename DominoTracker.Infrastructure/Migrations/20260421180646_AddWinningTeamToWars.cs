using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DominoTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWinningTeamToWars : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "WinningTeamId",
                table: "Wars",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WinningTeamId",
                table: "Wars");
        }
    }
}
