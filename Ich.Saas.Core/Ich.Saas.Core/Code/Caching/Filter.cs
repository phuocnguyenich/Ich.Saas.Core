using System.Collections.Generic;
using Ich.Saas.Core.Code.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;

namespace Ich.Saas.Core.Code.Caching
{
    #region Interface

    public interface IFilter
    {
        public List<SelectListItem> TenantItems { get; }
        public List<SelectListItem> ErrorItems { get; }
    }

    #endregion

    #region Implementation

    public class Filter : IFilter
    {
        #region Dependency Injection
        
        private readonly IStringLocalizer<SharedResources> _localizer;

        public Filter(IStringLocalizer<SharedResources> localizer)
        {
            _localizer = localizer;
        }

        #endregion

        #region Filter

        public List<SelectListItem> TenantItems
        {
            get
            {
                var list = new List<SelectListItem>
                {
                    new SelectListItem {Value = "0", Text = _localizer["All Tenants"], Selected = true},
                    new SelectListItem {Value = "1", Text = _localizer["Active Tenants"]},
                    new SelectListItem {Value = "2", Text = _localizer["InActive Tenants"]}
                };

                return list;
            }
        }

        public List<SelectListItem> ErrorItems
        {
            get
            {
                var list = new List<SelectListItem>
                {
                    new SelectListItem {Value = "0", Text = _localizer["-- Select --"], Selected = true},
                    new SelectListItem {Value = "5", Text = _localizer["Delete 5 errors"]},
                    new SelectListItem {Value = "10", Text = _localizer["Delete 10 errors"]},
                    new SelectListItem {Value = "25", Text = _localizer["Delete 25 errors"]},
                    new SelectListItem {Value = "100", Text = _localizer["Delete 100 errors"]},
                    new SelectListItem {Value = "All", Text = _localizer["Delete All errors"]}
                };

                return list;
            }
        }

        #endregion
    }

    #endregion
}