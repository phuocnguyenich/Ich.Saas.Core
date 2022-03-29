using Ich.Saas.Core.Code.Extensions;
using Microsoft.AspNetCore.Http;
using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;

namespace Ich.Saas.Core.Code.Identity
{
    #region Interface

    public interface ICurrentUser
    {
        int? Id { get; }
        string FirstName { get; }
        string LastName { get; }
        string Email { get; }

        bool IsAuthenticated { get; }
        bool IsHost { get; }
        bool IsAdmin { get; }
        bool IsUser { get; }

        int LanguageId { get; }
        CultureInfo CultureInfo { get; }
        TimeZoneInfo TimeZoneInfo { get; }
    }

    #endregion
    
    #region Implementation

    public class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public bool IsAuthenticated { get { return _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated; } }
        public bool IsHost { get { return _httpContextAccessor.HttpContext.User.IsInRole("Host"); } }
        public bool IsAdmin { get { return _httpContextAccessor.HttpContext.User.IsInRole("Admin"); } }
        public bool IsUser { get { return _httpContextAccessor.HttpContext.User.IsInRole("User"); } }

        public int? Id { get { return GetClaim(ClaimTypes.UserId).GetId(); } }
        public string FirstName { get { return GetClaim(ClaimTypes.FirstName); } }
        public string LastName { get { return GetClaim(ClaimTypes.LastName); } }
        public string Email { get { return GetClaim(ClaimTypes.Email); } }

        public int LanguageId { get { return GetClaim(ClaimTypes.LanguageId).GetInt(1); } }

        public CultureInfo CultureInfo { get { return new CultureInfo(GetClaim(ClaimTypes.LocaleName) ?? "en-US"); } }
        public TimeZoneInfo TimeZoneInfo { get { return TimeZoneInfo.FindSystemTimeZoneById(GetClaim(ClaimTypes.TimeZoneName) ?? "Central Standard Time"); } }

        private string GetClaim(string name)
        {
            var claims = _httpContextAccessor.HttpContext.User.Claims.Where(c => c.Type == name);
            var enumerable = claims as Claim[] ?? claims.ToArray();
            if (enumerable.Any())
                return enumerable.First().Value;

            return null;
        }
    }
    #endregion
}
