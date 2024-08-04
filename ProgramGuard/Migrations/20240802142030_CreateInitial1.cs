using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProgramGuard.Migrations
{
    /// <inheritdoc />
    public partial class CreateInitial1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CHangeDetail",
                table: "ChangeLogs",
                newName: "ChangeDetail");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ChangeDetail",
                table: "ChangeLogs",
                newName: "CHangeDetail");
        }
    }
}
