using ExamManagement.Models;

namespace ExamManagement.Repositories
{
    public interface ITeacherQuestionRepository
    {
        // create
        Task<Question> CreateQuestionAsync(Question question);
        // read all and read by id
        Task<IEnumerable<Question>> GetAllQuestionsAsync();
        Task<Question> GetQuestionByIdAsync(int id);
        // update
        Task<Question> UpdateQuestionAsync(Question question);
        // delete
        Task<Question> DeleteQuestionAsync(int id);
    }
}
