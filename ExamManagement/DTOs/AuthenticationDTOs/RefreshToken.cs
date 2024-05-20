using ExamManagement.Data;

namespace ExamManagement.DTOs.AuthenticationDTOs
{
    public class RefreshToken
    {
        public int Id { get; set; } // Primary key (database auto-increment)
        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public bool IsExpired => DateTime.UtcNow >= Expires;
        public DateTime Created { get; set; }
        public string CreatedByIp { get; set; }
        public DateTime? Revoked { get; set; }
        public bool IsActive => Revoked == null && !IsExpired;

        // Foreign key to your ApplicationUser
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }

}
