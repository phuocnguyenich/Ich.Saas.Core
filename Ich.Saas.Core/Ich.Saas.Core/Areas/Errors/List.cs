using Ich.Saas.Core.Code.Pagination;

namespace Ich.Saas.Core.Areas.Errors
{
    public class List : PagedModel<Detail>
    {
        public List() { Sort = "LastName"; }

        public string DeleteCount { get; set; }
    }
}