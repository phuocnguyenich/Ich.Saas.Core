using Ich.Saas.Core.Code.Pagination;

namespace Ich.Saas.Core.Areas.Enrollments
{
    public class List : PagedModel<Detail>
    {
        public List() { Sort = "EnrollNumber"; }

        public int? StudentId { get; set; }
        public int? ClassId { get; set; }
        public int? CourseId { get; set; }
        public string AmountPaidFrom { get; set; }
        public string AmountPaidThru { get; set; }
        public string Status { get; set; }
    }
}