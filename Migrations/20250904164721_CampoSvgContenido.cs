using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoogleRuta.Migrations
{
    /// <inheritdoc />
    public partial class CampoSvgContenido : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IconoSvgContent",
                table: "ElementTypes",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IconoSvgContent",
                table: "ElementTypes");
        }
    }
}
