namespace ExamManagement.DTOs.AuthenticationDTOs
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Token { get; set; }
        public string Message { get; set; }
        public IList<string> Roles { get; set; }

        public LoginResponse(bool success, string token, string message, IList<string> roles)
        {
            Success = success;
            Token = token;
            Message = message;
            Roles = roles;
        }
    }
}
