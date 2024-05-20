using ExamManagement.DTOs.AuthenticationDTOs;
using static ExamManagement.DTOs.AuthenticationDTOs.ServiceResponses;
namespace ExamManagement.Contracts
{
    public interface IUserAccount
    {
        Task<GeneralResponse> CreateAccount(UserDTO userDTO);
        Task<GeneralResponse> UpdateAccountAsync(string userId, UserDTO updateUserDTO);
        Task<GeneralResponse> CreateAdminAccount(UserDTO userDTO);
        Task<GeneralResponse> CreateSuperAdmin(UserDTO userDTO);
        Task<LoginResponse> LoginAccount(LoginDTO loginDTO);
        Task<GeneralResponse> LogoutAccount(string token);
        Task<GeneralResponse> CreateTeacherAccount(UserDTO teacherDTO);
        Task<GeneralResponse> CreateStudentAccount(UserDTO studentDTO);


        // New action to get users by role
        //Task<UserListResponse> GetUsersByRoleAsync(string roleName);
    }
}
