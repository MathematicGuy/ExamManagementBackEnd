using ExamManagement.Data;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ExamManagement.Models
{
    public class Teacher
    {
        public string TeacherId { get; set; }
        public string Name { get; set; }
        public string? Role { get; set; }
        public string? status { get; set; }

        // 1 to Many Relationship
        public ICollection<TeacherAssignment>? TeacherAssignments { get; set; }
    }
}


