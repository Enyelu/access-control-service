using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace access_control.infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AlterTableLocksAddOpenedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "OpenedAt",
                table: "Locks",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OpenedAt",
                table: "Locks");
        }
    }
}
