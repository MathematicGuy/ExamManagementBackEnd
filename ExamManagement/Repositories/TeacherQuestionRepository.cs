using ExamManagement.Data;
using ExamManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace ExamManagement.Repositories
{
    public class TeacherQuestionRepository : ITeacherQuestionRepository
    {
        private readonly SgsDbContext _context;

        public TeacherQuestionRepository(SgsDbContext context)
        {
            _context = context;
        }

        public async Task<Question> CreateQuestionAsync(Question question)
        {

            _context.Questions.Add(question);
            await _context.SaveChangesAsync();

            return question;
        }

        public async Task<Question> DeleteQuestionAsync(int id)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question != null)
            {
                _context.Questions.Remove(question);
                await _context.SaveChangesAsync();
            }
            return null;
        }

        public async Task<IEnumerable<Question>> GetAllQuestionsAsync()
        {
            return await _context.Questions.ToListAsync();
        }

        public async Task<Question> GetQuestionByIdAsync(int id)
        {
            return await _context.Questions.FindAsync(id);
        }

        public async Task<Question> UpdateQuestionAsync(Question question)
        {
            _context.Entry(question).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return question;
        }

    }
}
