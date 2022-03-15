using Ich.Saas.Core.Code.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ich.Saas.Core.Domain
{
    public partial class Language : IAuditable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime ChangedOn { get; set; }
        public int? ChangedBy { get; set; }

        public virtual ICollection<Tenant> Tenants { get; set; } = new HashSet<Tenant>();
        public virtual ICollection<Translate> Translates { get; set; } = new HashSet<Translate>();
        public virtual ICollection<User> Users { get; set; } = new HashSet<User>();
    }
}
