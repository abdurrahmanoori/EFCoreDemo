using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EFCoreDemo.Migrations
{
    public partial class InitalMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ENT_ACTIVITY",
                columns: table => new
                {
                    ENT_ACTIVITY_NO = table.Column<byte>(type: "INTEGER", nullable: false),
                    ENT_ACTIVITY_DESC = table.Column<string>(type: "TEXT", maxLength: 190, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ENT_ACTIVITY", x => x.ENT_ACTIVITY_NO);
                });

            migrationBuilder.CreateTable(
                name: "TAX_PAYER",
                columns: table => new
                {
                    TAX_PAYER_NO = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TAX_PAYER_NAME = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TAX_PAYER", x => x.TAX_PAYER_NO);
                });

            migrationBuilder.CreateTable(
                name: "ENTERPRISE",
                columns: table => new
                {
                    ENTERPRISE_NO = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ENTERPRISE_NAME = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    TAX_PAYER_NO = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ENTERPRISE", x => x.ENTERPRISE_NO);
                    table.ForeignKey(
                        name: "FK_ENTERPRISE_TAX_PAYER_TAX_PAYER_NO",
                        column: x => x.TAX_PAYER_NO,
                        principalTable: "TAX_PAYER",
                        principalColumn: "TAX_PAYER_NO",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ENT_BUS_ACT",
                columns: table => new
                {
                    ENT_ACTIVITY_NO = table.Column<byte>(type: "INTEGER", nullable: false),
                    ENTERPRISE_NO = table.Column<long>(type: "INTEGER", nullable: false),
                    MAIN_ACTIVITY_FL = table.Column<string>(type: "TEXT", maxLength: 1, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ENT_BUS_ACT", x => new { x.ENTERPRISE_NO, x.ENT_ACTIVITY_NO });
                    table.ForeignKey(
                        name: "FK_ENT_BUS_ACT_ENT_ACTIVITY_ENT_ACTIVITY_NO",
                        column: x => x.ENT_ACTIVITY_NO,
                        principalTable: "ENT_ACTIVITY",
                        principalColumn: "ENT_ACTIVITY_NO",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ENT_BUS_ACT_ENTERPRISE_ENTERPRISE_NO",
                        column: x => x.ENTERPRISE_NO,
                        principalTable: "ENTERPRISE",
                        principalColumn: "ENTERPRISE_NO",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ENT_BUS_ACT_ENT_ACTIVITY_NO",
                table: "ENT_BUS_ACT",
                column: "ENT_ACTIVITY_NO");

            migrationBuilder.CreateIndex(
                name: "IX_ENTERPRISE_TAX_PAYER_NO",
                table: "ENTERPRISE",
                column: "TAX_PAYER_NO");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ENT_BUS_ACT");

            migrationBuilder.DropTable(
                name: "ENT_ACTIVITY");

            migrationBuilder.DropTable(
                name: "ENTERPRISE");

            migrationBuilder.DropTable(
                name: "TAX_PAYER");
        }
    }
}
