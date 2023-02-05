using Microsoft.EntityFrameworkCore.Migrations;

namespace WebProject.Migrations
{
    public partial class updatedthesissubjectmodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ThesisRequests",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Topic = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    ThesisSubjectId = table.Column<int>(nullable: false),
                    StudentId = table.Column<int>(nullable: false),
                    ProfessorId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThesisRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThesisRequests_Professors_ProfessorId",
                        column: x => x.ProfessorId,
                        principalTable: "Professors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ThesisRequests_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ThesisRequests_ThesisSubjects_ThesisSubjectId",
                        column: x => x.ThesisSubjectId,
                        principalTable: "ThesisSubjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ThesisRequests_ProfessorId",
                table: "ThesisRequests",
                column: "ProfessorId");

            migrationBuilder.CreateIndex(
                name: "IX_ThesisRequests_StudentId",
                table: "ThesisRequests",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_ThesisRequests_ThesisSubjectId",
                table: "ThesisRequests",
                column: "ThesisSubjectId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ThesisRequests");
        }
    }
}
