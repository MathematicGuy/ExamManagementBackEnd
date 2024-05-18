using ExamManagement.Contracts;
using ExamManagement.Data;
using ExamManagement.DTOs.AuthenticationDTOs;
using ExamManagement.Models.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace ExamManagement.Controllers
{

    [ApiController]
    [Route("api/[controller]")]

    public class AccountController : ControllerBase
    {
        private readonly IUserAccount userAccount;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(IUserAccount userAccount, UserManager<ApplicationUser> userManager)
        {
            this.userAccount = userAccount;
            _userManager = userManager;
        }

        [HttpPost("CreateSuperAdmin")]
        public async Task<IActionResult> CreateSuperAdmin(UserDTO userDTO)
        {
            var result = await userAccount.CreateSuperAdmin(userDTO); // Assuming this returns IdentityResult

            if (result.Succeeded)
            {
                return Ok(); // Or return Ok with relevant data
            }
            else
            {
                return BadRequest(new ErrorResponse
                {
                    Message = "Failed to create admin user.",
                });
            }
        }

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
        [Authorize(Roles = "Admin")] // Restrict to existing SuperAdmin only
        //[Authorize(Policy = "SuperAdminOnly")] // Apply to specific action
        public async Task<IActionResult> CreateAdmin(UserDTO adminDTO)
        {
            var response = await userAccount.CreateAdminAccount(adminDTO);
            return Ok(response);
        }
    }
}



//{
//"email": "king@gmail.com",
//"password": "King@123",
//}


//{
//"id": "100",
//  "name": "Ha",
//  "email": "Ha@gmail.com",
//  "password": "Ha@123",
//  "confirmPassword": "Ha@123"
//}

//{
  //"email": "superadmin2@gmail.com",
  //"password": "Super2@123"
//}