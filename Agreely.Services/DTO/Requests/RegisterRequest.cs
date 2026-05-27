using System.ComponentModel.DataAnnotations;

namespace Agreely.Services.DTO.Requests
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(50, ErrorMessage = "Password cannot exceed 50 characters.")]
        public string Password { get; set; } = string.Empty;
    }
}
