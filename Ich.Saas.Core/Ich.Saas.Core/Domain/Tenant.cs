using Ich.Saas.Core.Code.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ich.Saas.Core.Domain
{
    public partial class Tenant : IAuditable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ContactPerson { get; set; }
        public string ContactEmail { get; set; }
        public string Color { get; set; }
        public int TimeZoneId { get; set; }
        public int LocaleId { get; set; }
        public int LanguageId { get; set; }
        public int CurrencyId { get; set; }
        public int TotalUsers { get; set; }
        public bool? IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime ChangedOn { get; set; }
        public int? ChangedBy { get; set; }

        public virtual Currency Currency { get; set; }
        public virtual Language Language { get; set; }
        public virtual Locale Locale { get; set; }
        public virtual TimeZone TimeZone { get; set; }
        public virtual ICollection<User> Users { get; set; } = new HashSet<User>();
    }
}
