using ExamManagement.Data;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace ExamManagement.Models.Domains
{
    public class StudentAssignment
    {
        [Required]
        public int StudentId { get; set; }

        [Required]
        public int AssignmentId { get; set; }
        // I want to save the sum of Assignment's Questions TotalPoints into AssignmentTotalPoint  


        // Many to Many relation. "Student" M-1 + 1-M "Assignment" = M-M
        public Student Student { get; set; }
        public Assignment Assignments { get; set; }
    }
}
