using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EFCoreDemo.Migrations
{
    public partial class addcoll : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "collClasses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    ActivityId = table.Column<byte>(type: "INTEGER", nullable: true),
                    EnterpriseId = table.Column<long>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_collClasses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_collClasses_EnterpriseBusinessActivities_EnterpriseId_ActivityId",
                        columns: x => new { x.EnterpriseId, x.ActivityId },
                        principalTable: "EnterpriseBusinessActivities",
                        principalColumns: new[] { "EnterpriseId", "ActivityId" });
                    table.ForeignKey(
                        name: "FK_collClasses_Enterprises_EnterpriseId",
                        column: x => x.EnterpriseId,
                        principalTable: "Enterprises",
                        principalColumn: "EnterpriseId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_collClasses_EnterpriseId_ActivityId",
                table: "collClasses",
                columns: new[] { "EnterpriseId", "ActivityId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "collClasses");
        }
    }
}
