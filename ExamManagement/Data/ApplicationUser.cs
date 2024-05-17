using Microsoft.AspNetCore.Identity;

namespace ExamManagement.Data
{
    public class ApplicationUser : IdentityUser
    {
        //"id": "2",
        //"name": "Admin",
        //"email": "admin2@example.com",
        //"password": "Admin2@123",

        public string? Name { get; set; }
    }
}
