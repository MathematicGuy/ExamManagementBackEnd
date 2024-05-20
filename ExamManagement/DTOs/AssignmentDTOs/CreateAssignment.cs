using System.ComponentModel.DataAnnotations;
using ExamManagement.DTOs.QuestionDTOs;
using ExamManagement.Models;

namespace ExamManagement.DTOs.AssignmentDTOs
{

    public class CreateAssignment
    {
        //public int AssignmentId { get; set; } // If you need to track the ID
        
        [Required]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public DateTime PublishTime { get; set; }

        public DateTime? CloseTime { get; set; }

        //public string? Status { get; set; }
    }
}
