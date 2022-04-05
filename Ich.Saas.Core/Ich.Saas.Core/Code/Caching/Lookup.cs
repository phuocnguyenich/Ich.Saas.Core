using System.Collections.Generic;
using Ich.Saas.Core.Code.Identity;
using Ich.Saas.Core.Code.Localization;
using Ich.Saas.Core.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;

namespace Ich.Saas.Core.Code.Caching
{
    public interface ILookup
    {
        List<SelectListItem> TimeZoneItems { get; }
        List<SelectListItem> LocaleItems { get; }
        List<SelectListItem> LanguageItems { get; }
        List<SelectListItem> CurrencyItems { get; }
    }

    public class Lookup : ILookup
    {
        private readonly SaaSContext _db;
        private readonly ICache _cache;
        private readonly ICurrentTenant _currentTenant;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public Lookup(SaaSContext db, ICache cache, ICurrentTenant currentTenant, IStringLocalizer<SharedResources> localizer)
        {
            _db = db;
            _cache = cache;
            _currentTenant = currentTenant;
            _localizer = localizer;
        }
        
        // Dropdown selection list for timezones

        public List<SelectListItem> TimeZoneItems
        {
            get
            {
                var list = new List<SelectListItem>();
                list.Add(new SelectListItem { Value = "", Text = _localizer["-- Select --"], Selected = true });
                foreach (var timezone in _cache.TimeZones.Values)
                    list.Add(new SelectListItem { Value = timezone.Id.ToString(), Text = timezone.DisplayName });

                return list;
            }
        }
        
        // Dropdown selection list for locales

        public List<SelectListItem> LocaleItems
        {
            get
            {
                var list = new List<SelectListItem>();
                list.Add(new SelectListItem { Value = "", Text = _localizer["-- Select --"], Selected = true });
                foreach (var locale in _cache.Locales.Values)
                    list.Add(new SelectListItem { Value = locale.Id.ToString(), Text = locale.DisplayName });

                return list;
            }
        }

        // Dropdown selection list for languages

        public List<SelectListItem> LanguageItems
        {
            get
            {
                var list = new List<SelectListItem>();
                list.Add(new SelectListItem { Value = "", Text = _localizer["-- Select --"], Selected = true });
                foreach (var language in _cache.Languages.Values)
                    list.Add(new SelectListItem { Value = language.Id.ToString(), Text = language.Name });

                return list;
            }
        }

        // Dropdown selection list for currencies

        public List<SelectListItem> CurrencyItems
        {
            get
            {
                var list = new List<SelectListItem>();
                list.Add(new SelectListItem { Value = "", Text = _localizer["-- Select --"], Selected = true });
                foreach (var currency in _cache.Currencies.Values)
                    list.Add(new SelectListItem { Value = currency.Id.ToString(), Text = currency.Name });

                return list;
            }
        }
    }
}