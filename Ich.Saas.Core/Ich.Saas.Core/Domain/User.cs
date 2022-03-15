using Ich.Saas.Core.Code.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ich.Saas.Core.Domain
{
    public partial class User : IAuditable
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Role { get; set; }
        public string IdentityId { get; set; }
        public string IdentityName { get; set; }
        public int? TimeZoneId { get; set; }
        public int? LocaleId { get; set; }
        public int? LanguageId { get; set; }
        public bool? IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime ChangedOn { get; set; }
        public int? ChangedBy { get; set; }

        public virtual Language Language { get; set; }
        public virtual Locale Locale { get; set; }
        public virtual Tenant Tenant { get; set; }
        public virtual TimeZone TimeZone { get; set; }
        public virtual ICollection<Error> Errors { get; set; } = new HashSet<Error>();
    }
}
