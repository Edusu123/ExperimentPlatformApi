using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExperimentPlatformInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ApplyEntityConfigurations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Events_ExperimentId_UserId",
                table: "Events",
                columns: new[] { "ExperimentId", "UserId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Events_ExperimentId_UserId",
                table: "Events");
        }
    }
}
