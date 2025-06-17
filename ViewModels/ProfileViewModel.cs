using System.ComponentModel.DataAnnotations;

namespace Tomomo7.ViewModels
{
    public class ProfileViewModel
    {
        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Phone]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [StringLength(255)]
        public string? Address { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string? OldPassword { get; set; }

        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "New password must be at least 6 characters long.")]
        [Display(Name = "New Password")]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string? ConfirmNewPassword { get; set; }

        [Required(ErrorMessage = "Password is required to delete your account.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm with your password")]
        public string? PasswordForDelete { get; set; }
    }
}
