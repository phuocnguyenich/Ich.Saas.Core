using Ich.Saas.Core.Code.Identity;
using Ich.Saas.Core.Domain;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ich.Saas.Core.Code.Caching
{
    #region Interface

    public interface ICache
    {
        Dictionary<int, Domain.TimeZone> TimeZones { get; }
        Dictionary<int, Locale> Locales { get; }
        Dictionary<int, Language> Languages { get; }
        Dictionary<int, Currency> Currencies { get; }

        Dictionary<int, Country> Countries { get; }
        Dictionary<int, Department> Departments { get; }
        Dictionary<int, Instructor> Instructors { get; }

        Dictionary<string, string> Translations { get; }

        void ClearCountries();
        void ClearInstructors();
        void ClearDepartments();
        void ClearTranslations();

        void Clear();
    }

    #endregion

    #region Implementation

    public class Cache : ICache
    {
        #region Dependency Injection

        private readonly SaaSContext _db;
        private readonly ICurrentTenant _currentTenant;
        private readonly ICurrentUser _currentUser;
        private readonly IMemoryCache _memoryCache;

        public Cache(SaaSContext db, ICurrentTenant currentTenant, ICurrentUser currentUser, IMemoryCache memoryCache)
        {
            _db = db;
            _currentTenant = currentTenant;
            _currentUser = currentUser;
            _memoryCache = memoryCache;
        }

        #endregion

        #region Cache management

        private static readonly object locker = new object();

        private const string TimeZonesKey = nameof(TimeZonesKey);
        private const string LocalesKey = nameof(LocalesKey);
        private const string LanguagesKey = nameof(LanguagesKey);
        private const string CurrenciesKey = nameof(CurrenciesKey);

        private const string CountriesKey = nameof(CountriesKey);
        private const string DepartmentsKey = nameof(DepartmentsKey);
        private const string InstructorsKey = nameof(InstructorsKey);

        private const string TranslatesKey = nameof(TranslatesKey);

        // Keeps track of keys used

        private static readonly HashSet<string> UsedKeys = new HashSet<string>();

        #endregion

        #region TimeZones

        // ** Identity Map Pattern

        public Dictionary<int, Domain.TimeZone> TimeZones
        {
            get
            {
                // ** Lazy load pattern

                if (!(_memoryCache.Get(TimeZonesKey) is Dictionary<int, Domain.TimeZone> dictionary))
                {
                    lock (locker)
                    {
                        dictionary = _db.TimeZone.OrderByDescending(t => t.OffsetHours).ToDictionary(t => t.Id);
                        Add(TimeZonesKey, dictionary, DateTime.Now.AddHours(24));
                    }
                }

                return dictionary;
            }
        }


        // Clears timezone cache

        private void ClearTimeZones()
        {
            Clear(TimeZonesKey);
        }

        #endregion

        #region Locales

        public Dictionary<int, Locale> Locales
        {
            get
            {
                // ** Lazy load pattern

                if (!(_memoryCache.Get(LocalesKey) is Dictionary<int, Locale> dictionary))
                {
                    lock (locker)
                    {
                        dictionary = _db.Locale.OrderBy(l => l.DisplayName).ToDictionary(l => l.Id);
                        Add(LocalesKey, dictionary, DateTime.Now.AddHours(24));
                    }
                }

                return dictionary;
            }
        }


        // Clear Locale cache

        private void ClearLocales()
        {
            Clear(LocalesKey);
        }

        #endregion

        #region Languages

        public Dictionary<int, Language> Languages
        {
            get
            {
                // ** Lazy load pattern

                if (!(_memoryCache.Get(LanguagesKey) is Dictionary<int, Language> dictionary))
                {
                    lock (locker)
                    {
                        dictionary = _db.Language.OrderBy(l => l.Id).ToDictionary(l => l.Id);
                        Add(LanguagesKey, dictionary, DateTime.Now.AddHours(24));
                    }
                }

                return dictionary;
            }
        }


        // Clear Language cache

        private void ClearLanguages()
        {
            Clear(LanguagesKey);
        }

        #endregion

        #region Currencies

        public Dictionary<int, Currency> Currencies
        {
            get
            {
                // ** Lazy load pattern

                if (!(_memoryCache.Get(CurrenciesKey) is Dictionary<int, Currency> dictionary))
                {
                    lock (locker)
                    {
                        dictionary = _db.Currency.OrderBy(c => c.Id).ToDictionary(c => c.Id);
                        Add(CurrenciesKey, dictionary, DateTime.Now.AddHours(24));
                    }
                }

                return dictionary;
            }
        }

        // Clear currencies cache

        private void ClearCurrencies()
        {
            Clear(CurrenciesKey);
        }

        #endregion

        #region Countries -- tenant specific!

        public Dictionary<int, Country> Countries
        {
            get
            {
                // ** Lazy load pattern

                var key = CountriesKey + _currentTenant.Id;

                if (!(_memoryCache.Get(key) is Dictionary<int, Country> dictionary))
                {
                    lock (locker)
                    {
                        dictionary = _db.Country.Where(c => c.TenantId == _currentTenant.Id).OrderBy(c => c.Name).ToDictionary(c => c.Id);
                        Add(key, dictionary, DateTime.Now.AddHours(24));
                    }
                }

                return dictionary;
            }
        }

        // Clear countries cache

        public void ClearCountries()
        {
            Clear(CountriesKey + _currentTenant.Id);
        }

        #endregion

        #region Departments -- tenant specific!

        public Dictionary<int, Department> Departments
        {
            get
            {
                // ** Lazy load pattern

                var key = DepartmentsKey + _currentTenant.Id;

                if (!(_memoryCache.Get(key) is Dictionary<int, Department> dictionary))
                {
                    lock (locker)
                    {
                        dictionary = _db.Department.Where(d => d.TenantId == _currentTenant.Id).OrderBy(d => d.Name).ToDictionary(d => d.Id);
                        Add(key, dictionary, DateTime.Now.AddHours(24));
                    }
                }

                return dictionary;
            }
        }


        // Clear departments cache

        public void ClearDepartments()
        {
            Clear(DepartmentsKey + _currentTenant.Id);
        }

        #endregion

        #region Instructors -- tenant specific!

        public Dictionary<int, Instructor> Instructors
        {
            get
            {
                // ** Lazy load pattern

                var key = InstructorsKey + _currentTenant.Id;

                if (!(_memoryCache.Get(key) is Dictionary<int, Instructor> dictionary))
                {
                    lock (locker)
                    {
                        dictionary = _db.Instructor.Where(i => i.TenantId == _currentTenant.Id).OrderBy(i => i.LastName).ToDictionary(i => i.Id);
                        Add(key, dictionary, DateTime.Now.AddHours(24));
                    }
                }

                return dictionary;
            }
        }

        public void ClearInstructors()
        {
            Clear(InstructorsKey + _currentTenant.Id);
        }

        #endregion

        #region Translations -- language specific!

        public Dictionary<string, string> Translations
        {
            get
            {
                // ** Lazy load pattern

                if (!(_memoryCache.Get(TranslatesKey) is Dictionary<int, Dictionary<string, string>> dictionaries))
                {
                    lock (locker)
                    {
                        dictionaries = new Dictionary<int, Dictionary<string, string>>();
                        foreach (var language in _db.Language)
                            dictionaries.Add(language.Id, new Dictionary<string, string>());

                        foreach (var translate in _db.Translate)
                            dictionaries[translate.LanguageId].Add(translate.Key, translate.Value);

                        Add(TranslatesKey, dictionaries, DateTime.Now.AddHours(24));
                    }
                }

                return dictionaries[_currentUser.LanguageId];
            }
        }


        // Clear resources cache

        public void ClearTranslations()
        {
            Clear(TranslatesKey);
        }

        #endregion

        #region Cache Helpers

        // clears single cache entry

        private void Clear(string key)
        {
            lock (locker)
            {
                _memoryCache.Remove(key);

                UsedKeys.Remove(key);
            }
        }

        // clears entire cache

        public void Clear()
        {
            // only host is allowed to clear entire cache

            if (!_currentUser.IsHost) return;

            lock (locker)
            {
                foreach (var usedKey in UsedKeys)
                    _memoryCache.Remove(usedKey);

                UsedKeys.Clear();
            }
        }

        // add to cache

        private void Add(string key, object value, DateTimeOffset expiration)
        {
            _memoryCache.Set(key, value, new MemoryCacheEntryOptions().SetAbsoluteExpiration(expiration));

            UsedKeys.Add(key);
        }

        #endregion
    }

    #endregion

}
