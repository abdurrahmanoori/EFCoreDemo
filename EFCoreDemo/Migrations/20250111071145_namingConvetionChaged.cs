﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EFCoreDemo.Migrations
{
    public partial class namingConvetionChaged : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ENTERPRISE_TAX_PAYER_TAX_PAYER_NO",
                table: "ENTERPRISE");

            migrationBuilder.DropTable(
                name: "ENT_BUS_ACT");

            migrationBuilder.DropTable(
                name: "TAX_PAYER");

            migrationBuilder.DropTable(
                name: "ENT_ACTIVITY");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ENTERPRISE",
                table: "ENTERPRISE");

            migrationBuilder.DropColumn(
                name: "ENTERPRISE_NAME",
                table: "ENTERPRISE");

            migrationBuilder.RenameTable(
                name: "ENTERPRISE",
                newName: "Enterprises");

            migrationBuilder.RenameColumn(
                name: "TAX_PAYER_NO",
                table: "Enterprises",
                newName: "TaxPayerId");

            migrationBuilder.RenameColumn(
                name: "ENTERPRISE_NO",
                table: "Enterprises",
                newName: "EnterpriseId");

            migrationBuilder.RenameIndex(
                name: "IX_ENTERPRISE_TAX_PAYER_NO",
                table: "Enterprises",
                newName: "IX_Enterprises_TaxPayerId");

            migrationBuilder.AddColumn<string>(
                name: "EnterpriseName",
                table: "Enterprises",
                type: "TEXT",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Enterprises",
                table: "Enterprises",
                column: "EnterpriseId");

            migrationBuilder.CreateTable(
                name: "Activities",
                columns: table => new
                {
                    ActivityId = table.Column<byte>(type: "INTEGER", nullable: false),
                    ActivityDescription = table.Column<string>(type: "TEXT", maxLength: 190, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activities", x => x.ActivityId);
                });

            migrationBuilder.CreateTable(
                name: "TaxPayers",
                columns: table => new
                {
                    TaxPayerId = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TaxPayerName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxPayers", x => x.TaxPayerId);
                });

            migrationBuilder.CreateTable(
                name: "EnterpriseBusinessActivities",
                columns: table => new
                {
                    ActivityId = table.Column<byte>(type: "INTEGER", nullable: false),
                    EnterpriseId = table.Column<long>(type: "INTEGER", nullable: false),
                    MainActivityFlag = table.Column<string>(type: "TEXT", maxLength: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnterpriseBusinessActivities", x => new { x.EnterpriseId, x.ActivityId });
                    table.ForeignKey(
                        name: "FK_EnterpriseBusinessActivities_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "ActivityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EnterpriseBusinessActivities_Enterprises_EnterpriseId",
                        column: x => x.EnterpriseId,
                        principalTable: "Enterprises",
                        principalColumn: "EnterpriseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EnterpriseBusinessActivities_ActivityId",
                table: "EnterpriseBusinessActivities",
                column: "ActivityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Enterprises_TaxPayers_TaxPayerId",
                table: "Enterprises",
                column: "TaxPayerId",
                principalTable: "TaxPayers",
                principalColumn: "TaxPayerId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enterprises_TaxPayers_TaxPayerId",
                table: "Enterprises");

            migrationBuilder.DropTable(
                name: "EnterpriseBusinessActivities");

            migrationBuilder.DropTable(
                name: "TaxPayers");

            migrationBuilder.DropTable(
                name: "Activities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Enterprises",
                table: "Enterprises");

            migrationBuilder.DropColumn(
                name: "EnterpriseName",
                table: "Enterprises");

            migrationBuilder.RenameTable(
                name: "Enterprises",
                newName: "ENTERPRISE");

            migrationBuilder.RenameColumn(
                name: "TaxPayerId",
                table: "ENTERPRISE",
                newName: "TAX_PAYER_NO");

            migrationBuilder.RenameColumn(
                name: "EnterpriseId",
                table: "ENTERPRISE",
                newName: "ENTERPRISE_NO");

            migrationBuilder.RenameIndex(
                name: "IX_Enterprises_TaxPayerId",
                table: "ENTERPRISE",
                newName: "IX_ENTERPRISE_TAX_PAYER_NO");

            migrationBuilder.AddColumn<string>(
                name: "ENTERPRISE_NAME",
                table: "ENTERPRISE",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ENTERPRISE",
                table: "ENTERPRISE",
                column: "ENTERPRISE_NO");

            migrationBuilder.CreateTable(
                name: "ENT_ACTIVITY",
                columns: table => new
                {
                    ENT_ACTIVITY_NO = table.Column<byte>(type: "INTEGER", nullable: false),
                    ENT_ACTIVITY_DESC = table.Column<string>(type: "TEXT", nullable: true)
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
                    TAX_PAYER_NAME = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TAX_PAYER", x => x.TAX_PAYER_NO);
                });

            migrationBuilder.CreateTable(
                name: "ENT_BUS_ACT",
                columns: table => new
                {
                    ENTERPRISE_NO = table.Column<long>(type: "INTEGER", nullable: false),
                    ENT_ACTIVITY_NO = table.Column<byte>(type: "INTEGER", nullable: false),
                    MAIN_ACTIVITY_FL = table.Column<string>(type: "TEXT", nullable: true)
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

            migrationBuilder.AddForeignKey(
                name: "FK_ENTERPRISE_TAX_PAYER_TAX_PAYER_NO",
                table: "ENTERPRISE",
                column: "TAX_PAYER_NO",
                principalTable: "TAX_PAYER",
                principalColumn: "TAX_PAYER_NO",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
