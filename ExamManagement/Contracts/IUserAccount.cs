using ExamManagement.DTOs;
using static ExamManagement.DTOs.ServiceResponses;
namespace ExamManagement.Contracts
{
    public interface IUserAccount
    {
        Task<GeneralResponse> CreateAccount(UserDTO userDTO);
        Task<LoginResponse> LoginAccount(LoginDTO loginDTO);
    }
}
