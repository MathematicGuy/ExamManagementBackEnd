using System.ComponentModel.DataAnnotations;

namespace ExamManagement.Models.ViewModels
{
    public class CreateQuestion
    {
        //public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }

        public int? TotalPoints { get; set; }

        public string? AnswerFileURL { get; set; }

        public string? Status { get; set; }

    }

}
