using Ich.Saas.Core.Code.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ich.Saas.Core.Domain
{
    public partial class Instructor : IAuditable
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Alias { get; set; }
        public DateTime? HireDate { get; set; }
        public bool IsFullTime { get; set; }
        public decimal? Salary { get; set; }
        public int TotalCourses { get; set; }

        public DateTime CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime ChangedOn { get; set; }
        public int? ChangedBy { get; set; }

        public virtual ICollection<Course> Courses { get; set; } = new HashSet<Course>();
    }
}
