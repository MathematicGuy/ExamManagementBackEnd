using Microsoft.EntityFrameworkCore;
using ExamManagement.Data;
using ExamManagement.Models;

namespace ExamManagement.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly SgsDbContext _context;

        public StudentRepository(SgsDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Student>> GetAllStudentsAsync()
        {
            return await _context.Student.ToListAsync();
        }

        public async Task<Student?> GetStudentByIdAsync(int id)
        {
            return await _context.Student.FindAsync(id);
        }

        public async Task<Student> CreateStudentAsync(Student student)
        {
            _context.Student.Add(student);
            await _context.SaveChangesAsync();
            return student;
        }

        public async Task UpdateStudentAsync(Student student)
        {
            _context.Entry(student).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteStudentAsync(int id)
        {
            var student = await _context.Student.FindAsync(id);
            if (student == null)
            {
                // Handle case where student is not found (e.g., throw an exception or log it)
                return;
            }
            _context.Student.Remove(student);
            await _context.SaveChangesAsync();
        }
    }
}
