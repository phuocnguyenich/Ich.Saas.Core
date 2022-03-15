using Ich.Saas.Core.Code.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ich.Saas.Core.Domain
{
    public partial class Course : IAuditable
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string CourseNumber { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public int NumDays { get; set; }
        public int DepartmentId { get; set; }
        public string Department { get; set; }
        public int? InstructorId { get; set; }
        public string Instructor { get; set; }
        public decimal Fee { get; set; }
        public int TotalClasses { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime ChangedOn { get; set; }
        public int? ChangedBy { get; set; }

        public virtual Department DepartmentNavigation { get; set; }
        public virtual Instructor InstructorNavigation { get; set; }
        public virtual ICollection<Class> Classes { get; set; } = new HashSet<Class>();
    }
}
