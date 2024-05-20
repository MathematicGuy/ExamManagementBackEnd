using ExamManagement.DTOs.AssignmentDTOs;
using ExamManagement.Models;

namespace ExamManagement.Repositories
{
    public interface IAssignmentRepository
    {
        //Task<Assignment> CreateAssignmentAsync(Assignment assignment, List<Question> questions, string teacherId);
        Task<CreateAssignment> GetAssignmentByIdAsync(int id); // Include Questions in ViewModel
        Task<Assignment> CreateAssignmentAsync(CreateAssignment assignment, TeacherAssignmentCreate teacherAssignment);
        Task AddQuestionToAssignmentAsync(string teacherId, int assignmentId, int question);
    }
}
