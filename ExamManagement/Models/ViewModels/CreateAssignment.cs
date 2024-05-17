using System.ComponentModel.DataAnnotations;
using ExamManagement.Models.Domains;

namespace ExamManagement.Models.ViewModels
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


        public List<CreateQuestion> Questions { get; set; } = new();

        // You can add a method to calculate total points based on QuestionViewModels
        public int? GetTotalPoints() => Questions.Sum(q => q.TotalPoints);
    }
}
