using System.ComponentModel.DataAnnotations;

namespace ExamManagement.DTOs.AssignmentDTOs
{
    public class teacherAssignment
    {
        [Required]
        public string? TeacherId { get; set; }

        [Required]
        public int AssignmentId { get; set; }

    }
}
