namespace ExamManagement.DTOs.AuthenticationDTOs
{
    public record UserSession(string? Id, string? Name, string? Email, string? Role);
}
