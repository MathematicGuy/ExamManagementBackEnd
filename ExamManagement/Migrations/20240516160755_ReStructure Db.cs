using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExamManagement.Migrations
{
    /// <inheritdoc />
    public partial class ReStructureDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignmentTotalPoints",
                table: "StudentAssignments");

            migrationBuilder.DropColumn(
                name: "Evaluation",
                table: "Student");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "Student");

            migrationBuilder.DropColumn(
                name: "ScoreAI",
                table: "Student");

            migrationBuilder.RenameColumn(
                name: "Note",
                table: "Student",
                newName: "Role");

            migrationBuilder.AddColumn<int>(
                name: "AssignmentTotalPoints",
                table: "Assignments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Evaluation",
                table: "AssignmentQuestions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "AssignmentQuestions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<float>(
                name: "Score",
                table: "AssignmentQuestions",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "ScoreAI",
                table: "AssignmentQuestions",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignmentTotalPoints",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "Evaluation",
                table: "AssignmentQuestions");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "AssignmentQuestions");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "AssignmentQuestions");

            migrationBuilder.DropColumn(
                name: "ScoreAI",
                table: "AssignmentQuestions");

            migrationBuilder.RenameColumn(
                name: "Role",
                table: "Student",
                newName: "Note");

            migrationBuilder.AddColumn<int>(
                name: "AssignmentTotalPoints",
                table: "StudentAssignments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Evaluation",
                table: "Student",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Score",
                table: "Student",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "ScoreAI",
                table: "Student",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }
    }
}
