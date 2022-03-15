using Ich.Saas.Core.Code.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ich.Saas.Core.Domain
{
    public partial class Currency : IAuditable
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
        public string Symbol { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime ChangedOn { get; set; }
        public int? ChangedBy { get; set; }

        public virtual ICollection<Tenant> Tenants { get; set; } = new HashSet<Tenant>();
    }
}
