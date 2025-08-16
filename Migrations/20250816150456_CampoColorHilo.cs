using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoogleRuta.Migrations
{
    /// <inheritdoc />
    public partial class CampoColorHilo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StrandColorsJson",
                table: "Nodos",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StrandColorsJson",
                table: "Nodos");
        }
    }
}
