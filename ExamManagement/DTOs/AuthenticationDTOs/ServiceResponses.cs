namespace ExamManagement.DTOs.AuthenticationDTOs
{
    public class ServiceResponses
    {
        public record class GeneralResponse(bool Flag, string Message)
        {
            internal readonly bool Succeeded;
        }

    }
}
