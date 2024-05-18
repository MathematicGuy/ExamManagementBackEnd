namespace ExamManagement.DTOs.AuthenticationDTOs
{
    public class UserSession
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public IList<string> Roles { get; set; }

        public UserSession(string userId, string name, string email, IList<string> roles)
        {
            UserId = userId;
            Name = name;
            Email = email;
            Roles = roles;
        }
    }
}
