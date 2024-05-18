using ExamManagement.Contracts;
using ExamManagement.DTOs.AuthenticationDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace ExamManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(IUserAccount userAccount) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserDTO userDTO)
        {
            var response = await userAccount.CreateAccount(userDTO);
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            var response = await userAccount.LoginAccount(loginDTO);
            return Ok(response);
        }

        [HttpPost("CreateAdmin")]
        [Authorize(Roles = "Admin")] // Restrict to existing admins only
        public async Task<IActionResult> CreateAdmin(UserDTO adminDTO)
        {

            var response = await userAccount.CreateAdminAccount(adminDTO);
            return Ok(response);
        }
    }
}


//{
    //"id": "2",
    //"name": "king",
    //"email": "king@gmail.com",
    //"password": "King@123",
    //"confirmPassword": "King@123"
//}
