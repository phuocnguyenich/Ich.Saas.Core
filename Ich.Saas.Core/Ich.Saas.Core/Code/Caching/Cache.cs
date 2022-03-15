using Ich.Saas.Core.Code.Identity;
using Ich.Saas.Core.Domain;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ich.Saas.Core.Code.Caching
{
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

    public class Cache
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

        // add to cache

        private void Add(string key, object value, DateTimeOffset expiration)
        {
            _memoryCache.Set(key, value, new MemoryCacheEntryOptions().SetAbsoluteExpiration(expiration));

            UsedKeys.Add(key);
        }

        #endregion
    }
}
