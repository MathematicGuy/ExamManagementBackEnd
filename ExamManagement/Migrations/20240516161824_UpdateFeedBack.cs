using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExamManagement.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFeedBack : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "Context",
                table: "FeedBacks",
                newName: "Evaluation");

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "FeedBacks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<float>(
                name: "Score",
                table: "FeedBacks",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "ScoreAI",
                table: "FeedBacks",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Note",
                table: "FeedBacks");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "FeedBacks");

            migrationBuilder.DropColumn(
                name: "ScoreAI",
                table: "FeedBacks");

            migrationBuilder.RenameColumn(
                name: "Evaluation",
                table: "FeedBacks",
                newName: "Context");

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
    }
}
