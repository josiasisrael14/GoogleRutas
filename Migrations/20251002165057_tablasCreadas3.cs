using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoogleRuta.Migrations
{
    /// <inheritdoc />
    public partial class tablasCreadas3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Routers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IPAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Routers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Switchs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalPorts = table.Column<int>(type: "int", nullable: false),
                    GroupCount = table.Column<int>(type: "int", nullable: false),
                    PortsPerGroup = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Switchs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SwitchPorts",
                columns: table => new
                {
                    SwitchsId = table.Column<int>(type: "int", nullable: false),
                    PortNumber = table.Column<int>(type: "int", nullable: false),
                    GroupNumber = table.Column<int>(type: "int", nullable: false),
                    RouterId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SwitchPorts", x => new { x.SwitchsId, x.GroupNumber, x.PortNumber });
                    table.ForeignKey(
                        name: "FK_SwitchPorts_Routers_RouterId",
                        column: x => x.RouterId,
                        principalTable: "Routers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SwitchPorts_Switchs_SwitchsId",
                        column: x => x.SwitchsId,
                        principalTable: "Switchs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SwitchPorts_RouterId",
                table: "SwitchPorts",
                column: "RouterId",
                unique: true,
                filter: "[RouterId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SwitchPorts");

            migrationBuilder.DropTable(
                name: "Routers");

            migrationBuilder.DropTable(
                name: "Switchs");
        }
    }
}
