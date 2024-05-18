using ExamManagement.Data;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

public static class DataSeeder
{
    public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        if (!await roleManager.RoleExistsAsync("Student"))
        {
            await roleManager.CreateAsync(new IdentityRole("Student"));
        }
        if (!await roleManager.RoleExistsAsync("Teacher"))
        {
            await roleManager.CreateAsync(new IdentityRole("Teacher"));
        }
        if (!await roleManager.RoleExistsAsync("SuperAdmin"))
        {
            await roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
        }
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }
    }

    public static async Task SeedSuperAdminAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        var superAdmin = new ApplicationUser
        {
            Name = "Super", // Custom property if you have added it
            UserName = "superadmin@gmail.com",
            NormalizedUserName = "SUPERADMIN@GMAIL.COM",
            Email = "superadmin@gmail.com",
            NormalizedEmail = "SUPERADMIN@GMAIL.COM",
            EmailConfirmed = true,
            PhoneNumber = "+1234567890",
            PhoneNumberConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString("D"),
            ConcurrencyStamp = Guid.NewGuid().ToString("D"),
            TwoFactorEnabled = false,
            LockoutEnabled = true,
            LockoutEnd = null,
            AccessFailedCount = 0
        };

        //var user = await userManager.FindByEmailAsync(superAdmin.Email);
        if (await userManager.FindByEmailAsync(superAdmin.Email) == null)
        {
            var result = await userManager.CreateAsync(superAdmin, "Super@123");
            if (result.Succeeded)
            {
                await userManager.AddToRolesAsync(superAdmin, new[] { "Student", "Teacher", "SuperAdmin", "Admin" });
            }
        }
    }
}
