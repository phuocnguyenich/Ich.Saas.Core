using System.ComponentModel.DataAnnotations;

namespace Ich.Saas.Core.Areas.Accounts
{
    public class Password
    {
        [Required(ErrorMessage = "Current Password is required")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }
        
        [Required(ErrorMessage = "New Password is required")]
        [StringLength(50, MinimumLength = 7, ErrorMessage = "Min password length is 7 characters")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        
        [Required(ErrorMessage = "Confirm Password is required")]
        [StringLength(50, MinimumLength = 7, ErrorMessage = "Min password length is 7 characters")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "New Password and Confirmation Password do not match")]
        public string ConfirmPassword { get; set; }
    }
}