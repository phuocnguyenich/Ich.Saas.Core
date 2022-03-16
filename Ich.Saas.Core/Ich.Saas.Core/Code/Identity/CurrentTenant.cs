using Ich.Saas.Core.Code.Extensions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ich.Saas.Core.Code.Identity
{
    #region Interface

    public interface ICurrentTenant
    {
        int? Id { get; }
        string Name { get; }

        string Color { get; }
        string CurrencySymbol { get; }
    }

    #endregion

    #region Implementation

    public class CurrentTenant : ICurrentTenant
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentTenant(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? Id
        {
            get
            {
                return GetClaim(ClaimTypes.TenantId).GetId();
            }
        }

        public string Name
        {
            get
            {
                return GetClaim(ClaimTypes.TenantName);
            }
        }

        public string Color
        {
            get
            {
                return GetClaim(ClaimTypes.Color);
            }
        }

        public string CurrencySymbol
        {
            get
            {
                return GetClaim(ClaimTypes.CurrencySymbol);
            }
        }

        private string GetClaim(string name)
        {
            var claims = _httpContextAccessor.HttpContext.User.Claims.Where(c => c.Type == name);
            if (claims != null && claims.Any())
                return claims.First().Value;

            return null;
        }
    }

    #endregion
}
