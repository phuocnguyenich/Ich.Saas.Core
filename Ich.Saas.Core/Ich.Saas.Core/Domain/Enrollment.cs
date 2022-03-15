using Ich.Saas.Core.Code.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ich.Saas.Core.Domain
{
    public partial class Enrollment : IAuditable
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string EnrollNumber { get; set; }
        public DateTime EnrollDate { get; set; }
        public int StudentId { get; set; }
        public string Student { get; set; }
        public int ClassId { get; set; }
        public string Class { get; set; }
        public int CourseId { get; set; }
        public string Course { get; set; }
        public string Status { get; set; }
        public decimal Fee { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal? AverageGrade { get; set; }
        public int TotalQuizzes { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime ChangedOn { get; set; }
        public int? ChangedBy { get; set; }

        public virtual Class ClassNavigation { get; set; }
        public virtual Student StudentNavigation { get; set; }
        public virtual ICollection<Quiz> Quizzes { get; set; } = new HashSet<Quiz>();
    }
}
