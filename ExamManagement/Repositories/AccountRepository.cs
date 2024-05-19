using ExamManagement.Contracts;
using ExamManagement.Data;
using ExamManagement.DTOs.AuthenticationDTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web.Mvc;
using static ExamManagement.DTOs.AuthenticationDTOs.ServiceResponses;
namespace ExamManagement.Repositories
{
    public class AccountRepository(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration config) : IUserAccount
    {

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


        // In your AccountRepository class (or wherever you implement IUserAccount)
        //public async Task<UserListResponse> GetUsersByRoleAsync(string roleName)
        //{
        //    var users = await userManager.GetUsersInRoleAsync(roleName);

        //    if (users == null)
        //    {
        //        return new UserListResponse(false, "No user found", null);
        //    }

        //    var viewUserDTOs = users.Select(user => new ViewUserDTO
        //    {
        //        Id = user.Id,
        //        Name = user.Name,   
        //        Email = user.Email,
        //        PhoneNumber = user.PhoneNumber                
        //    });

        //    return new UserListResponse(true, "Users retrieved successfully.", viewUserDTOs);
        //}



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

    }
}
