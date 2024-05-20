using ExamManagement.DTOs.QuestionDTOs;
using ExamManagement.Models;
using System.ComponentModel.DataAnnotations;

namespace ExamManagement.DTOs.AssignmentDTOs
{
    public class ViewDetailAssignment
    {
        public int Id { get; set; } // If you need to track the ID

        public int TeacherId { get; set; }


        [Required]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public DateTime PublishTime { get; set; }

        public DateTime? CloseTime { get; set; }

        public List<ViewQuestion>? Questions { get; set; } = new();

        public Teacher Teacher { get; set; }

        // This property calculates the total points from the questions
        public int TotalPoints { get; set; }
        public string? Status { get; set; }

        // Method to calculate total points based on questions
        public int? GetTotalPoints() => Questions.Sum(q => q.TotalPoints);
    }
}
