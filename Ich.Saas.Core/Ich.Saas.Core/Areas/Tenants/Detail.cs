using Ich.Saas.Core.Code;

namespace Ich.Saas.Core.Areas.Tenants
{
    public class Detail : BaseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CreatedDate { get; set; }
        public string ContactPerson { get; set; }
        public string ContactEmail { get; set; }
        public string Logo { get; set; }
        public string Color { get; set; }
        public int? TimeZoneId { get; set; }
        public string TimeZone { get; set; }
        public int? LocaleId { get; set; }
        public string Locale { get; set; }
        public int? LanguageId { get; set; }
        public string Language { get; set; }
        public int? CurrencyId { get; set; }
        public string Currency { get; set; }
        public int TotalUsers { get; set; }
        public bool IsActive { get; set; }
    }
}