using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoodTracker.Api.Common.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MoodEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Mood = table.Column<string>(type: "TEXT", maxLength: 16, nullable: false),
                    Note = table.Column<string>(type: "TEXT", nullable: true),
                    LoggedAt = table.Column<long>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<long>(type: "INTEGER", nullable: false),
                    UpdatedAt = table.Column<long>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoodEntries", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MoodEntries_LoggedAt",
                table: "MoodEntries",
                column: "LoggedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MoodEntries");
        }
    }
}
