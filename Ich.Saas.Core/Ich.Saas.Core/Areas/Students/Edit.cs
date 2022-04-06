using System.ComponentModel.DataAnnotations;
using Ich.Saas.Core.Code;

namespace Ich.Saas.Core.Areas.Students
{
    public class Edit : BaseModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Alias is required")]
        public string Alias { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string City { get; set; }

        [Required(ErrorMessage = "Country is required")]
        public int? CountryId { get; set; }

        public string BirthDate { get; set; }
        public string Gender { get; set; }
        public string FullName { get; set; }
    }
}