using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExamManagement.Migrations
{
    /// <inheritdoc />
    public partial class modifyStudentModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Student");

            migrationBuilder.RenameColumn(
                name: "Role",
                table: "Student",
                newName: "Note");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Student",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
