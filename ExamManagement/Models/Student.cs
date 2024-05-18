using ExamManagement.Data;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ExamManagement.Models
{
    public class Student
    {
        public int StudentId { get; set; }
        [Required]
        public string? Name { get; set; }
        public string? Role { get; set; }
        public string? Status { get; set; }

        // 1 to Many Relationship
        public ICollection<StudentAssignment>? StudentAssignments { get; set; }
    }
}


