using ExamManagement.Data;
using ExamManagement.Models;

namespace ExamManagement.Repositories
{
    public interface IStudentRepository
    {
        Task<IEnumerable<Student>> GetAllStudentsAsync();
        Task<Student?> GetStudentByIdAsync(string id);
        Task<Student> CreateStudentAsync(Student student);
        Task UpdateStudentAsync(Student student);
        //Task DeleteStudentAsync(string id);
    }
}
