using ExamManagement.DTOs.AssignmentDTOs;
using ExamManagement.Models;

namespace ExamManagement.Repositories
{
    public interface IAssignmentRepository
    {
        //Task<Assignment> CreateAssignmentAsync(Assignment assignment, List<Question> questions, string teacherId);
        Task<CreateAssignment?> GetAssignmentByIdAsync(int id); // Include Questions in ViewModel
        Task<bool> IsTeacherAuthorized(int assignmentId, int teacherId);
        Task<Assignment> CreateAssignmentAsync(Assignment assignment, Models.TeacherAssignment teacherAssignment);
        Task AddQuestionToAssignmentAsync(int assignmentId, Question question); // Modified method
    }
}
