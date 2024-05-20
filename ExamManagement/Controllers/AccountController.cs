using ExamManagement.Contracts;
using ExamManagement.Data;
using ExamManagement.DTOs.AuthenticationDTOs;
using ExamManagement.Models.Errors;
using ExamManagement.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ExamManagement.Controllers
{

    [ApiController]
    [Route("api/[controller]")]

    public class AccountController : ControllerBase
    {
        private readonly IUserAccount _userAccount;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AppDbContext _context;
        private readonly ITokenService _tokenService;

        public AccountController(
            IUserAccount userAccount,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            AppDbContext context,
            ITokenService tokenService
            )
        {
            _userAccount = userAccount;
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost("CreateSuperAdmin")]
        public async Task<IActionResult> CreateSuperAdmin(UserDTO userDTO)
        {
            var result = await _userAccount.CreateSuperAdmin(userDTO); // Assuming this returns IdentityResult

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
            var response = await _userAccount.CreateAdminAccount(adminDTO);
            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("CreateTeacher")]
        public async Task<IActionResult> CreateTeacher(UserDTO teacherDTO)
        {

            var result = await _userAccount.CreateTeacherAccount(teacherDTO);

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

            var result = await _userAccount.CreateStudentAccount(studentDTO);

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
            var response = await _userAccount.CreateAccount(userDTO);
            return Ok(response);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
   
            var response = await _userAccount.LoginAccount(loginDTO);
            return Ok(response);
        }

        [Authorize]
        [HttpPut("UpdateAccount")]
        public async Task<IActionResult> UpdateAccount(UserDTO updateUserDTO)
        {
            if (updateUserDTO == null)
                return BadRequest(new ErrorResponse { Message = "Model is empty" });

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await _userAccount.UpdateAccountAsync(userId, updateUserDTO);

            if (!response.Flag)
                return BadRequest(new ErrorResponse { Message = response.Message });

            return Ok(response);
        }

        //[HttpPost("logout")]
        //[Authorize]
        //public async Task<IActionResult> Logout()
        //{
        //    // Retrieve the token from the Authorization header
        //    var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        //    var result = await _userAccount.LogoutAccount(token);
        //    if (result.Flag)
        //    {
        //        return Ok(result.Message);
        //    }

        //    return BadRequest(result.Message);
        //}

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
        {
            if (string.IsNullOrEmpty(request.RefreshToken))
            {
                return BadRequest("Invalid request");
            }

            await _tokenService.InvalidateRefreshTokenAsync(request.RefreshToken);
            return Ok(new { message = "Logout successful" });
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
            //var userEmail = await userManager.FindByEmailAsync(loginDTO.Email);

            Console.Write(user.Id);
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
                TokenExpiration = tokenExpiry, // Include token expiration time
                UserRoles = await _userManager.GetRolesAsync(user),
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


public class LogoutRequest{
    public string RefreshToken { get; set; }
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