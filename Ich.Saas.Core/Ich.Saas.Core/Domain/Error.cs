using Ich.Saas.Core.Code.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ich.Saas.Core.Domain
{
    public partial class Error : IAuditable
    {
        public int Id { get; set; }
        public int? TenantId { get; set; }
        public int? UserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
        public string IpAddress { get; set; }
        public string Url { get; set; }
        public string HttpReferer { get; set; }
        public string UserAgent { get; set; }
        public string ServerName { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime ChangedOn { get; set; }
        public int? ChangedBy { get; set; }

        public virtual User User { get; set; }
        public virtual Tenant Tenant { get; set; }
    }
}
