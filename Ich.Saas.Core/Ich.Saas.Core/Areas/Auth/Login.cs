using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ich.Saas.Core.Areas.Auth
{
    public class Login
    {
        [Required(ErrorMessage = "Email is required.")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }

        public List<_User> Users = new List<_User>();
    }
}