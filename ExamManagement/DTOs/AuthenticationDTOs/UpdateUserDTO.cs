using System.ComponentModel.DataAnnotations;

namespace ExamManagement.DTOs.AuthenticationDTOs
{
    public class UpdateUserDTO
    {
        //public string? Id { get; set; } = string.Empty;
        [Required]
        public string? Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }
    }
}
