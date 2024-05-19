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
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }
        if (!await roleManager.RoleExistsAsync("SuperAdmin"))
        {
            await roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
        }

    }

    public static async Task SeedSuperAdminAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        var superAdmin = new ApplicationUser
        {
            Name = "Super", // Custom property if you have added it
            UserName = "superadmin2@gmail.com",
            NormalizedUserName = "SUPERADMIN2@GMAIL.COM",
            Email = "superadmin2@gmail.com",
            NormalizedEmail = "SUPERADMIN2@GMAIL.COM",
            EmailConfirmed = true,
            PhoneNumber = "+0363965123",
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
                await userManager.AddToRolesAsync(superAdmin, new[] { "SuperAdmin", "Admin", "Student", "Teacher"});
            }
        }
    }
}
