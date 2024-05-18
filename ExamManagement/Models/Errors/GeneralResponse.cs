using Microsoft.AspNetCore.Identity;

namespace ExamManagement.Models.Errors 
{
    public class GeneralResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public object? Data { get; set; }
        public IdentityResult? IdentityResult { get; set; } // Add this property

        // Constructor to initialize IsSuccess and Message
        public GeneralResponse(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }

        public GeneralResponse(IdentityResult result)
        {
            IsSuccess = result.Succeeded;
            Message = result.Succeeded ? "Operation successful" : "Operation failed";
            IdentityResult = result; // Optional: Store the IdentityResult for more details
        }
    }
}
