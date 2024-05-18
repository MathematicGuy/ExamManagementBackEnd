using ExamManagement.DTOs.AuthenticationDTOs;
using static ExamManagement.DTOs.AuthenticationDTOs.ServiceResponses;
namespace ExamManagement.Contracts
{
    public interface IUserAccount
    {
        Task<GeneralResponse> CreateAccount(UserDTO userDTO);
        Task<LoginResponse> LoginAccount(LoginDTO loginDTO);
    }
}
