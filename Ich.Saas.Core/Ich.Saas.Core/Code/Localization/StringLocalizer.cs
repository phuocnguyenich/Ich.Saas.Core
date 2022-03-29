using System;
using System.Collections.Generic;
using System.Globalization;
using Ich.Saas.Core.Code.Caching;
using Ich.Saas.Core.Code.Infrastructure;
using Microsoft.Extensions.Localization;

namespace Ich.Saas.Core.Code.Localization
{
    public class StringLocalizer : IStringLocalizer
    {
        private readonly ICache _cache = ServiceLocator.Resolve<ICache>();


        public LocalizedString this[string name]
        {
            get
            {
                return new LocalizedString(name, Translate(name));
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                return new LocalizedString(name, Translate(name));
            }
        }

        private string Translate(string key)
        {
            var resources = _cache.Translations;
            if (resources == null) return key;
            return resources.TryGetValue(key, out var result) ? result : key;
        }
        
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            throw new NotImplementedException();
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}