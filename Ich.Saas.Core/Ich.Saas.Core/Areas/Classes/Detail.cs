using System.Collections.Generic;
using Ich.Saas.Core.Code;

namespace Ich.Saas.Core.Areas.Classes
{
    public class Detail : BaseModel
    {
        public int Id { get; set; }
        public int? TenantId { get; set; }
        public string ClassNumber { get; set; }
        public int? CourseId { get; set; }
        public string Course { get; set; }
        public string Location { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int MaxEnrollments { get; set; }
        public int TotalEnrollments { get; set; }

        // Related List

        public string Tab { get; set; } = "details";

        public List<_Enrollment> Enrollments { get; set; } = new List<_Enrollment>();

    }
}