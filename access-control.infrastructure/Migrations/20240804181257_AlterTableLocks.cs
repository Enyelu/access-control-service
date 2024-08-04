using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace access_control.infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AlterTableLocks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsLocked",
                table: "Locks",
                newName: "IsOpen");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsOpen",
                table: "Locks",
                newName: "IsLocked");
        }
    }
}
