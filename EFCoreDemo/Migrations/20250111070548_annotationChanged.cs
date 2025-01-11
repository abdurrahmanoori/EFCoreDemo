using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EFCoreDemo.Migrations
{
    public partial class annotationChanged : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TAX_PAYER_NAME",
                table: "TAX_PAYER",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "ENTERPRISE_NAME",
                table: "ENTERPRISE",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "MAIN_ACTIVITY_FL",
                table: "ENT_BUS_ACT",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 1);

            migrationBuilder.AlterColumn<string>(
                name: "ENT_ACTIVITY_DESC",
                table: "ENT_ACTIVITY",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 190);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TAX_PAYER_NAME",
                table: "TAX_PAYER",
                type: "TEXT",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ENTERPRISE_NAME",
                table: "ENTERPRISE",
                type: "TEXT",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MAIN_ACTIVITY_FL",
                table: "ENT_BUS_ACT",
                type: "TEXT",
                maxLength: 1,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ENT_ACTIVITY_DESC",
                table: "ENT_ACTIVITY",
                type: "TEXT",
                maxLength: 190,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);
        }
    }
}
