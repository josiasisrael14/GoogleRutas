using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoogleRuta.Migrations
{
    /// <inheritdoc />
    public partial class CampoSegment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "CoordinateBs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SegmentId",
                table: "CoordinateBs",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "CoordinateBs");

            migrationBuilder.DropColumn(
                name: "SegmentId",
                table: "CoordinateBs");
        }
    }
}
