using Microsoft.AspNetCore.Identity;

namespace ExamManagement.Data
{
    public class ApplicationUser : IdentityUser
    {
        //"id": "2",
        //"name": "Admin",
        //"email": "admin@example.com",
        //"password": "Admin@123",

        public string? Name { get; set; }
    }
}
