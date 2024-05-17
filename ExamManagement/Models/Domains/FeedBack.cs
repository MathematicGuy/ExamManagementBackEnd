using System.ComponentModel.DataAnnotations;

namespace ExamManagement.Models.Domains
{
    public class FeedBack
    {
        public int Id { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [Required(ErrorMessage = "Score must be integer")]
        [RegularExpression("^[1-9]\\d*$", ErrorMessage = "ScoreAI must be a positive integer")]
        public float? Score { get; set; }

        // Teacher Evaluvation
        public string? Evaluation { get; set; }

        [Required(ErrorMessage = "ScoreAI is required")]
        [RegularExpression("^[1-9]\\d*$", ErrorMessage = "ScoreAI must be a positive integer")]
        public float? ScoreAI { get; set; }

        [Required(ErrorMessage = "ScoreAI is required")]
        [RegularExpression("^[1-9]\\d*$", ErrorMessage = "ScoreAI must be a positive integer")]
        public string? Note { get; set; }



        // 1 to Many Relationship
        public Question? QuestionResponse { get; set; }
    }
}
