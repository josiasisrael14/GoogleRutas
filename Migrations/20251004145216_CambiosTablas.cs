using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoogleRuta.Migrations
{
    /// <inheritdoc />
    public partial class CambiosTablas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SwitchPorts_RouterId",
                table: "SwitchPorts");

            migrationBuilder.AlterColumn<int>(
                name: "RouterId",
                table: "SwitchPorts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "SwitchPorts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SwitchPorts_RouterId",
                table: "SwitchPorts",
                column: "RouterId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SwitchPorts_RouterId",
                table: "SwitchPorts");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "SwitchPorts");

            migrationBuilder.AlterColumn<int>(
                name: "RouterId",
                table: "SwitchPorts",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_SwitchPorts_RouterId",
                table: "SwitchPorts",
                column: "RouterId",
                unique: true,
                filter: "[RouterId] IS NOT NULL");
        }
    }
}
