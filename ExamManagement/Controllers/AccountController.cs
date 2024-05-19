using ExamManagement.Contracts;
using ExamManagement.Data;
using ExamManagement.DTOs.AuthenticationDTOs;
using ExamManagement.Models.Errors;
using ExamManagement.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;


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

        [Authorize(Roles = "Admin")] // Restrict to existing SuperAdmin only
        [HttpPost("CreateAdmin")]
        //[Authorize(Policy = "SuperAdminOnly")] // Apply to specific action
        public async Task<IActionResult> CreateAdmin(UserDTO adminDTO)
        {
            var response = await userAccount.CreateAdminAccount(adminDTO);
            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("CreateTeacher")]
        public async Task<IActionResult> CreateTeacher(UserDTO teacherDTO)
        {

            var result = await userAccount.CreateTeacherAccount(teacherDTO);

            if (result.Flag) // Check if the operation was successful
            {
                return Ok(result); // Return success response
            }
            else
            {
                // Create and return an ErrorResponse from the GeneralResponse
                return BadRequest(new ErrorResponse
                {
                    Message = result.Message,
                    Details = new[] { "Failed to create Teacher user." } // You can customize details here if needed
                });
            }
        }


        //{
        //  "name": "Student",
        //  "email": "student@gmail.com",
        //  "password": "Std@123",
        //  "confirmPassword": "Std@123"
        //}
        [Authorize(Roles = "Admin")]
        [HttpPost("CreateStudent")]
        public async Task<IActionResult> CreateStudent(UserDTO studentDTO)
        {

            var result = await userAccount.CreateStudentAccount(studentDTO);

            if (result.Flag) // Check if the operation was successful
            {
                return Ok(result); // Return success response
            }
            else
            {
                // Create and return an ErrorResponse from the GeneralResponse
                return BadRequest(new ErrorResponse
                {
                    Message = result.Message,
                    Details = new[] { "Failed to create Student user." } // You can customize details here if needed
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


        [HttpGet("Account")]
        [Authorize] // Requires user to be logged in. reutrn 401 if not logged in
        public async Task<IActionResult> GetCurrentUserInfo()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get User ID

            //if (string.IsNullOrEmpty(userId))
            //{
            //    return Unauthorized(new { message = "User must Logged in to see your personal information" });
            //}

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            // Get token expiration time
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(Request.Headers["Authorization"].ToString().Replace("Bearer ", ""));
            var tokenExpiry = token.ValidTo;

            var userInfo = new
            {
                user.Id,
                user.Name,
                user.UserName,
                user.Email,
                user.PhoneNumber,
                user.EmailConfirmed,
                user.PasswordHash,
                user.SecurityStamp,
                user.NormalizedEmail,
                user.NormalizedUserName,
                user.ConcurrencyStamp,
                user.LockoutEnd,
                user.TwoFactorEnabled,
                user.LockoutEnabled,
                user.AccessFailedCount,
                Token = token.RawData,
                TokenExpiration = tokenExpiry  // Include token expiration time
            };

            return Ok(userInfo);
        }


        [HttpGet("GetUserByRole{roleName}")] // Example: api/userByRole/Student
        //[Authorize(Roles = "Admin, SuperAdmi  n")] // Restrict access to authorized roles
        public async Task<IActionResult> GetUsersByRole(string roleName)
        {
            var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);

            if (usersInRole == null || !usersInRole.Any())
            {
                return NotFound(new { message = $"No users found with role '{roleName}'" });
            }

            var userDTOs = usersInRole.Select(user => new UserDTO
            {
                Name = user.Name,
                Email = user.Email
            });

            return Ok(userDTOs);
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