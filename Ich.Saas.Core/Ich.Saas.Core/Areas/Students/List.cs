using Ich.Saas.Core.Code.Pagination;

namespace Ich.Saas.Core.Areas.Students
{
    public class List : PagedModel<Detail>
    {
        public List()
        {
            Sort = "LastName";
        }
        
        // Advanced filter values

        public string City { get; set; }
        public int? CountryId { get; set; }
        public string BirthDayFrom { get; set; }
        public string BirthDayThru { get; set; }
        public int? TotalEnrollments { get; set; }
    }
}