using Ich.Saas.Core.Code.Pagination;

namespace Ich.Saas.Core.Areas.Tenants
{
    public class List : PagedModel<Detail>
    {
        public List() { Sort = "-Name"; }
    }
}