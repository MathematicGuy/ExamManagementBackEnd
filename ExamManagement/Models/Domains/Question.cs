using System.ComponentModel.DataAnnotations;

namespace ExamManagement.Models.Domains
{
    public class Question : IValidatableObject
    {
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        public string? Description { get; set; }

        public int? TotalPoints { get; set; }

        public string? AnswerFileURL { get; set; }

        public string? Status { get; set; }


        // 1 to many relationship with AssignmentQuestion
        public ICollection<FeedBack> QuestionFeedback { get; set; }

        public ICollection<AssignmentQuestion> AssignmentQuestion { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (TotalPoints < 0)
            {
                yield return new ValidationResult("Total points cannot be negative.", new[] { nameof(TotalPoints) });
            }
        }
    }
}
