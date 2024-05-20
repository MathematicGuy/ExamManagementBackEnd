using ExamManagement.DTOs.AuthenticationDTOs;
using Microsoft.AspNetCore.Identity;

namespace ExamManagement.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string? Name { get; set; }

        // Navigation property for refresh tokens
        public ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}
