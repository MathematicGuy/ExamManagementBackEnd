using ExamManagement.Contracts;
using ExamManagement.Data;
using ExamManagement.DTOs.AuthenticationDTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static ExamManagement.DTOs.AuthenticationDTOs.ServiceResponses;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

namespace ExamManagement.Repositories
{
    public class AccountRepository : IUserAccount
    {
        private readonly AppDbContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration config;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ITokenService tokenService;

        public AccountRepository(
            AppDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration config,
            SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService
         )
        {
            this.context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.config = config;
            this.signInManager = signInManager;
            this.tokenService = tokenService;
        }

        public async Task<GeneralResponse> CreateAccount(UserDTO userDTO)
        {
            if (userDTO is null) return new GeneralResponse(false, "Model is empty");

            if (userDTO == null)
                return new GeneralResponse(false, "Model is empty");

            if (userDTO.Password != userDTO.ConfirmPassword)
                return new GeneralResponse(false, "Passwords do not match");


            var newUser = new ApplicationUser()
            {
                Name = userDTO.Name,
                Email = userDTO.Email,
                PasswordHash = userDTO.Password,
                UserName = userDTO.Email
            };

            var user = await userManager.FindByEmailAsync(newUser.Email);
            if (user is not null) return new GeneralResponse(false, "User registered already");

            var createUser = await userManager.CreateAsync(newUser!, userDTO.Password);
            if (!createUser.Succeeded) return new GeneralResponse(false, "Error occured.. please try again");

            //Assign Default Role : Admin to first registrater; rest is user
            var checkAdmin = await roleManager.FindByNameAsync("Admin");
            if (checkAdmin is null)
            {
                await roleManager.CreateAsync(new IdentityRole() { Name = "Admin" });
                await userManager.AddToRoleAsync(newUser, "Admin");
                return new GeneralResponse(true, "Account Created");
            }
            else
            {
                var checkUser = await roleManager.FindByNameAsync("User");
                if (checkUser is null)
                    await roleManager.CreateAsync(new IdentityRole() { Name = "User" });

                await userManager.AddToRoleAsync(newUser, "User");
                return new GeneralResponse(true, "Account Created");
            }
        }

        public async Task<GeneralResponse> UpdateAccountAsync(string userId, UserDTO updateUserDTO)
        {
            if (updateUserDTO == null)
                return new GeneralResponse(false, "Model is empty");

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return new GeneralResponse(false, "User not found");

            user.Name = updateUserDTO.Name;
            user.Email = updateUserDTO.Email;
            user.UserName = updateUserDTO.Email;

            var updateResult = await userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return new GeneralResponse(false, "Failed to update user details");

            if (!string.IsNullOrWhiteSpace(updateUserDTO.Password) && updateUserDTO.Password == updateUserDTO.ConfirmPassword)
            {
                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                var passwordResult = await userManager.ResetPasswordAsync(user, token, updateUserDTO.Password);
                if (!passwordResult.Succeeded)
                    return new GeneralResponse(false, "Failed to update password");
            }

            return new GeneralResponse(true, "Account updated successfully");
        }


        public async Task<LoginResponse> LoginAccount(LoginDTO loginDTO)
        {
            if (loginDTO == null)
                return new LoginResponse(false, null!, "Login container is empty", null!);

            var user = await userManager.FindByEmailAsync(loginDTO.Email);
            if (user == null)
                return new LoginResponse(false, null!, "User not found", null!);

            bool checkUserPasswords = await userManager.CheckPasswordAsync(user, loginDTO.Password);
            if (!checkUserPasswords)
                return new LoginResponse(false, null!, "Invalid email/password", null!);

            // Get the user's roles
            var userRoles = await userManager.GetRolesAsync(user);

            // Create UserSession with the roles
            var userSession = new UserSession(user.Id, user.Name, user.Email, userRoles);
            string token = GenerateToken(userSession);

            // Return the response including the roles
            return new LoginResponse(true, token, "Login completed", userRoles);
        }

 
        // Token for login and logout
        private string GenerateToken(UserSession user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email)
            };

            // Add roles to the claims
            claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var token = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<GeneralResponse> LogoutAccount(string token)
        {
            var user = await userManager.GetUserAsync(signInManager.Context.User);
            if (user != null)
            {
                await userManager.UpdateSecurityStampAsync(user);
            }
            //await tokenService.AddTokenToBlacklist(token);

            return new GeneralResponse(true, "Logout successful");
        }


        public async Task<GeneralResponse> CreateSuperAdmin(UserDTO superAdminDTO)
        {
            if (superAdminDTO is null) return new GeneralResponse(false, "Model is empty");

            var newSuperAdmin = new ApplicationUser()
            {
                Name = superAdminDTO.Name,
                Email = superAdminDTO.Email,
                PasswordHash = superAdminDTO.Password,
                UserName = superAdminDTO.Email
            };

            var existingUser = await userManager.FindByEmailAsync(newSuperAdmin.Email);
            if (existingUser is not null)
                return new GeneralResponse(false, "User already registered");

            var result = await userManager.CreateAsync(newSuperAdmin, superAdminDTO.Password);
            if (!result.Succeeded)
                return new GeneralResponse(false, "Error occurred while creating the account.");

            // Ensure the Admin role exists if not create and assign to the user
            var superAdminRole = await roleManager.FindByNameAsync("SuperAdmin");
            if (superAdminRole == null)
            {
                superAdminRole = new IdentityRole("SuperAdmin");
                await roleManager.CreateAsync(superAdminRole);
            }

            // Assign "User" and "Admin" roles to the SuperAdmin
            await userManager.AddToRoleAsync(newSuperAdmin, "User");
            await userManager.AddToRoleAsync(newSuperAdmin, "Admin");

            return new GeneralResponse(true, "Admin account created");
        }

        public async Task<GeneralResponse> CreateAdminAccount(UserDTO adminDTO)
        {
            if (adminDTO is null) return new GeneralResponse(false, "Model is empty");

            var newAdmin = new ApplicationUser()
            {
                Name = adminDTO.Name,
                Email = adminDTO.Email,
                PasswordHash = adminDTO.Password,
                UserName = adminDTO.Email
            };

            var existingUser = await userManager.FindByEmailAsync(newAdmin.Email);
            if (existingUser is not null)
                return new GeneralResponse(false, "User already registered");

            var result = await userManager.CreateAsync(newAdmin, adminDTO.Password);
            if (!result.Succeeded)
                return new GeneralResponse(false, "Error occurred while creating the account.");

            // Ensure the Admin role exists
            var adminRole = await roleManager.FindByNameAsync("Admin");
            if (adminRole == null)
            {
                adminRole = new IdentityRole("Admin");
                await roleManager.CreateAsync(adminRole);
            }

            // Add the user to the Admin role
            await userManager.AddToRoleAsync(newAdmin, "Admin");
            return new GeneralResponse(true, "Admin account created");
        }

        public async Task<GeneralResponse> CreateTeacherAccount(UserDTO teacherDTO)
        {
            if (teacherDTO is null)
                return new GeneralResponse(false, "Model is empty");

            var newTeacher = new ApplicationUser()
            {
                Name = teacherDTO.Name,
                Email = teacherDTO.Email,
                PasswordHash = teacherDTO.Password,
                UserName = teacherDTO.Email
            };

            var existingUser = await userManager.FindByEmailAsync(newTeacher.Email);
            if (existingUser is not null)
                return new GeneralResponse(false, "User already registered");

            var result = await userManager.CreateAsync(newTeacher, teacherDTO.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return new GeneralResponse(false, string.Join(", ", errors)); // Return error message with details
            }

            // Ensure the Teacher role exists
            var teacherRole = await roleManager.FindByNameAsync("Teacher");
            if (teacherRole == null)
            {
                teacherRole = new IdentityRole("Teacher");
                await roleManager.CreateAsync(teacherRole);
            }

            // Add the user to the Teacher role
            await userManager.AddToRoleAsync(newTeacher, "Teacher");
            return new GeneralResponse(true, "Teacher account created");
        }

        public async Task<GeneralResponse> CreateStudentAccount(UserDTO studentDTO)
        {
            if (studentDTO is null)
                return new GeneralResponse(false, "Model is empty");

            var newStudent = new ApplicationUser()
            {
                Name = studentDTO.Name,
                Email = studentDTO.Email,
                PasswordHash = studentDTO.Password,
                UserName = studentDTO.Email
            };

            var existingUser = await userManager.FindByEmailAsync(newStudent.Email);
            if (existingUser is not null)
                return new GeneralResponse(false, "User already registered");

            var result = await userManager.CreateAsync(newStudent, studentDTO.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return new GeneralResponse(false, string.Join(", ", errors));
            }

            // Ensure the Student role exists
            var studentRole = await roleManager.FindByNameAsync("Student");
            if (studentRole == null)
            {
                studentRole = new IdentityRole("Student");
                await roleManager.CreateAsync(studentRole);
            }

            // Add the user to the Student role
            await userManager.AddToRoleAsync(newStudent, "Student");
            return new GeneralResponse(true, "Student account created");
        }




        // Update Profile Features
    }

}
