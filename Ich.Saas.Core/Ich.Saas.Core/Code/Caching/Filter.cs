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

        #endregion
    }

    #endregion
}