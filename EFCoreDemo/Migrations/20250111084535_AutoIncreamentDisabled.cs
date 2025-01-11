using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EFCoreDemo.Migrations
{
    public partial class AutoIncreamentDisabled : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "TaxPayerId",
                table: "TaxPayers",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<long>(
                name: "EnterpriseId",
                table: "Enterprises",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "TaxPayerId",
                table: "TaxPayers",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<long>(
                name: "EnterpriseId",
                table: "Enterprises",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);
        }
    }
}
