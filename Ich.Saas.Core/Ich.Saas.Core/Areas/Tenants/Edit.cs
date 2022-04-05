using System.ComponentModel.DataAnnotations;
using Ich.Saas.Core.Code;

namespace Ich.Saas.Core.Areas.Tenants
{
    public class Edit : BaseModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }
        public string CreatedDate { get; set; }

        [Required(ErrorMessage = "Contact is required")]
        public string ContactPerson { get; set; }
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string ContactEmail { get; set; }
        public string Logo { get; set; }
        public string Color { get; set; }
        [Required(ErrorMessage = "Time Zone is required")]
        public int? TimeZoneId { get; set; }
        [Required(ErrorMessage = "Locale is required")]
        public int? LocaleId { get; set; }
        [Required(ErrorMessage = "Language is required")]
        public int? LanguageId { get; set; }
        [Required(ErrorMessage = "Currency is required")]
        public int? CurrencyId { get; set; }

        public int TotalUsers { get; set; }
        public bool IsActive { get; set; }
    }
}