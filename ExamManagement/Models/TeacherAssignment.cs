using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using ExamManagement.Data;

namespace ExamManagement.Models
{
    public class TeacherAssignment
    {

        [Required]
        public int TeacherId { get; set; }

        [Required]
        public int AssignmentId { get; set; }

        // TeacherAssignment` is a join table between `ApplicationUser` and `Assignment`
        // Many to 1
        public Teacher Teacher { get; set; }
        public Assignment Assignment { get; set; }
    }
}
