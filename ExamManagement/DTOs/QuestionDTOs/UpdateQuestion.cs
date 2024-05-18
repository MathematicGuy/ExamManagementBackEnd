using System.ComponentModel.DataAnnotations;

namespace ExamManagement.DTOs.QuestionDTOs
{
    // Saving Edit Model data then transfer it to List Model
    public class UpdateQuestion
    {

        [Required]
        public string? Name { get; set; }

        public string? Description { get; set; }

        public int? TotalPoints { get; set; }

        public string? AnswerFileURL { get; set; }

        public string? Status { get; set; }

    }
}
