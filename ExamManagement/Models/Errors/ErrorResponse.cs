namespace ExamManagement.Models.Errors
{
    public class ErrorResponse
    {
        public string Message { get; set; }
        public IEnumerable<string> Details { get; set; }
    }
}
