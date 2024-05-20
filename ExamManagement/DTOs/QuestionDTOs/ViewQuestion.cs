using System.ComponentModel.DataAnnotations;

namespace ExamManagement.DTOs.QuestionDTOs
{
    public class ViewQuestion
    {
        public int QuestionId { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Required]
        public int TotalPoints { get; set; }

        public string? AnswerFileURL { get; set; }

        public string? Status { get; set; }        
    }
}
