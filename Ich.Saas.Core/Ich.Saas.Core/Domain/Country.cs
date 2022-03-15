using Ich.Saas.Core.Code.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ich.Saas.Core.Domain
{
    public partial class Country : IAuditable
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime ChangedOn { get; set; }
        public int? ChangedBy { get; set; }

        public virtual ICollection<Student> Students { get; set; } = new HashSet<Student>();
    }
}
