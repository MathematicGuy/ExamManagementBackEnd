namespace ExamManagement.DTOs.AuthenticationDTOs
{
    // In your AuthenticationDTOs namespace
    public class UserListResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public IEnumerable<UserDTO> Users { get; set; }

        // Add a constructor that takes 3 arguments:
        public UserListResponse(bool success, string message, IEnumerable<UserDTO> users)
        {
            Success = success;
            Message = message;
            Users = users;
        }
    }

}