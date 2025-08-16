using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoogleRuta.Migrations
{
    /// <inheritdoc />
    public partial class CampoRotatioYZise : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Rotation",
                table: "Nodos",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "Size",
                table: "Nodos",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rotation",
                table: "Nodos");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "Nodos");
        }
    }
}
