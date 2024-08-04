using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace access_control.infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AlterTableEventLogAddTenantId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "EventLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "EventLogs");
        }
    }
}
