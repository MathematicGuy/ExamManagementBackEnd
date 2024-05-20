using System.ComponentModel.DataAnnotations;
using ExamManagement.DTOs.QuestionDTOs;
using ExamManagement.Models;

namespace ExamManagement.DTOs.AssignmentDTOs
{

    public class CreateAssignment
    {
        public int AssignmentId { get; set; } // If you need to track the ID

        [Required]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public DateTime PublishTime { get; set; }

        public DateTime? CloseTime { get; set; }

        public List<CreateQuestion>? Questions { get; set; } = new();

        // This property calculates the total points from the questions
        public int TotalPoints { get; set; }

        // Method to calculate total points based on questions
        public int? GetTotalPoints() => Questions.Sum(q => q.TotalPoints);
    }
}
