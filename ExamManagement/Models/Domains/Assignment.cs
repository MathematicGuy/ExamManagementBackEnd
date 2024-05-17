using ExamManagement.Data;
using System.ComponentModel.DataAnnotations;

namespace ExamManagement.Models.Domains
{
    public class Assignment
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)] // Example max length constraint
        public string Title { get; set; }

        public string? Description { get; set; }

        [Required]
        public DateTime PublishTime { get; set; }

        public DateTime? CloseTime { get; set; } // CloseTime is optional 

        public int? AssignmentTotalPoints { get; set; }

        [Required]
        public string Status { get; set; }

        // Many to 1 Teacher & Student User
        public ICollection<TeacherAssignment>? TeacherAssignments { get; set; } // Change to
        public ICollection<StudentAssignment>? StudentAssignments { get; set; } // Change to ICollection

        // 1 to many to QuestionResponse
        public ICollection<AssignmentQuestion>? AssignmentQuestions { get; set; }
    }
}
