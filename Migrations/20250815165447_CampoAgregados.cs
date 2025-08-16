using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoogleRuta.Migrations
{
    /// <inheritdoc />
    public partial class CampoAgregados : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DestinoPuntoIndex",
                table: "Connections",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OrigenPuntoIndex",
                table: "Connections",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DestinoPuntoIndex",
                table: "Connections");

            migrationBuilder.DropColumn(
                name: "OrigenPuntoIndex",
                table: "Connections");
        }
    }
}
