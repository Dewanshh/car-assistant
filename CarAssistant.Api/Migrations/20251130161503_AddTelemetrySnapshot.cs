using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarAssistant.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddTelemetrySnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Telemetry",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CarId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TimestampUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Rpm = table.Column<int>(type: "INTEGER", nullable: false),
                    SpeedKph = table.Column<double>(type: "REAL", nullable: false),
                    CoolantTempC = table.Column<double>(type: "REAL", nullable: false),
                    FuelLevelPercent = table.Column<double>(type: "REAL", nullable: false),
                    ActiveDtcCodes = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Telemetry", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Telemetry");
        }
    }
}
