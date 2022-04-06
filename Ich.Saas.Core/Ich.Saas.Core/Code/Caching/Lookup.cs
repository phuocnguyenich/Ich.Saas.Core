using System.Collections.Generic;
using System.Linq;
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
        List<SelectListItem> StudentTotalEnrollmentItems { get; }
        List<SelectListItem> StudentCityItems { get; }
        List<SelectListItem> StudentCountryItems { get; }
        List<SelectListItem> CountryItems { get; }
        List<SelectListItem> GenderItems { get; }
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

        public List<SelectListItem> StudentTotalEnrollmentItems
        {
            get
            {
                var list = new List<SelectListItem> {new SelectListItem { Value = null, Text = _localizer["-- Select --"], Selected = true }};
                
                for (int i = 0; i < 11; i++)
                    list.Add(new SelectListItem { Value = i.ToString(), Text = i.ToString() });

                return list;
            }
        }

        public List<SelectListItem> StudentCityItems
        {
            get
            {
                var list = new List<SelectListItem> {new SelectListItem { Value = "", Text = _localizer["-- Select --"], Selected = true }};

                var cities = _db.Student.Where(s => s.TenantId == _currentTenant.Id).Select(s => s.City).Distinct();

                foreach (var city in cities)
                    list.Add(new SelectListItem { Value = city, Text = city });

                return list;
            }
        }
        
        public List<SelectListItem> StudentCountryItems
        {
            get
            {
                var list = new List<SelectListItem> {new SelectListItem { Value = "", Text = _localizer["-- Select --"], Selected = true }};

                var countries = _db.Student.Where(s => s.TenantId == _currentTenant.Id).Select(s => new { s.CountryId, s.Country } ).Distinct();

                foreach (var country in countries)
                    list.Add(new SelectListItem { Value = country.CountryId.ToString(), Text = country.Country });

                return list;
            }
        }
        
        public List<SelectListItem> GenderItems
        {
            get
            {
                var list = new List<SelectListItem>
                {
                    new SelectListItem { Value = "", Text = _localizer["-- Select --"], Selected = true },
                    new SelectListItem { Value = "Male", Text = _localizer["Male"] },
                    new SelectListItem { Value = "Female", Text = _localizer["Female"] }
                };

                return list;
            }
        }
        
        public List<SelectListItem> CountryItems
        {
            get
            {
                var list = new List<SelectListItem>();
                list.Add(new SelectListItem { Value = "", Text = _localizer["-- Select --"], Selected = true });
                foreach (var country in _cache.Countries.Values)
                    list.Add(new SelectListItem { Value = country.Id.ToString(), Text = country.Name });

                return list;
            }
        }
    }
}