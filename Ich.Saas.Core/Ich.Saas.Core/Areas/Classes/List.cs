using Ich.Saas.Core.Code.Pagination;

namespace Ich.Saas.Core.Areas.Classes
{
    public class List : PagedModel<Detail>
    {
        public List() { Sort = "ClassNumber"; }
       
        public string CourseId { get; set; }
        public string Location { get; set; }
        public string StartDateFrom { get; set; }
        public string StartDateThru { get; set; }
        public string TotalEnrollmentsFrom { get; set; }
        public string TotalEnrollmentsThru { get; set; }
    }
}