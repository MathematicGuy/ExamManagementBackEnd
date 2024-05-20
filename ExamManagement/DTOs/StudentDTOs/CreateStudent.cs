using System.ComponentModel.DataAnnotations;

namespace ExamManagement.DTOs.StudentDTOs
{
    public class CreateStudent
    {
        public string StudentId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string? Name { get; set; }
        public string? Role { get; set; }
        public string? Status { get; set; }

    }

}