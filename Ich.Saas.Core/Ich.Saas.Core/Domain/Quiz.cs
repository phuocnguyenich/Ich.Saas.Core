using Ich.Saas.Core.Code.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ich.Saas.Core.Domain
{
    public partial class Quiz : IAuditable
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string QuizNumber { get; set; }
        public int EnrollmentId { get; set; }
        public string Enrollment { get; set; }
        public DateTime QuizDate { get; set; }
        public decimal Grade { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime ChangedOn { get; set; }
        public int? ChangedBy { get; set; }

        public virtual Enrollment EnrollmentNavigation { get; set; }
    }
}
