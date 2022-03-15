using Ich.Saas.Core.Code.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ich.Saas.Core.Domain
{
    public partial class Class : IAuditable
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string ClassNumber { get; set; }
        public int CourseId { get; set; }
        public string Course { get; set; }
        public string Location { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int MaxEnrollments { get; set; }
        public int TotalEnrollments { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime ChangedOn { get; set; }
        public int? ChangedBy { get; set; }

        public virtual Course CourseNavigation { get; set; }
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new HashSet<Enrollment>();
    }
}
