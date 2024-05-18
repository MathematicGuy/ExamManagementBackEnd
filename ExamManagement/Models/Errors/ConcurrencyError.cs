namespace ExamManagement.Models.Errors
{
    public class ConcurrencyError
    {
        public string PropertyName { get; set; }
        public string ErrorMessage { get; set; }
    }
}
