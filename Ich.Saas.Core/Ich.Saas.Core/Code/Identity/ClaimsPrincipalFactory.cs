using Ich.Saas.Core.Code.Caching;
using Ich.Saas.Core.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Ich.Saas.Core.Code.Identity
{
    // ** Factory Pattern

    public class ClaimsPrincipalFactory : UserClaimsPrincipalFactory<IdentityUser, IdentityRole>
    {
        private readonly SaaSContext _db;
        private readonly ICache _cache;

        public ClaimsPrincipalFactory(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager, 
            IOptions<IdentityOptions> optionsAccessor, 
            SaaSContext db, 
            ICache cache) : base(userManager, roleManager, optionsAccessor)
        {
            _db = db;
            _cache = cache;
        }

        // ** Factory Method Pattern

        public override async Task<ClaimsPrincipal> CreateAsync(IdentityUser appUser)
        {
            try
            {
                var principal = await base.CreateAsync(appUser);
                var identity = principal.Identity as ClaimsIdentity;

                var user = _db.User.AsNoTracking().Single(u => u.IdentityId == appUser.Id);

                var tenant = _db.Tenant.AsNoTracking().Single(t => t.Id == user.TenantId);

                var symbol = _cache.Currencies[tenant.CurrencyId].Symbol;

                // Add Tenant claims

                if (identity != null)
                {
                    identity.AddClaim(new Claim(ClaimTypes.TenantId, tenant.Id.ToString()));
                    identity.AddClaim(new Claim(ClaimTypes.TenantName, tenant.Name));
                    identity.AddClaim(new Claim(ClaimTypes.Color, tenant.Color));
                    identity.AddClaim(new Claim(ClaimTypes.CurrencySymbol, symbol));

                    // Add User claims

                    identity.AddClaim(new Claim(ClaimTypes.UserId, user.Id.ToString()));
                    identity.AddClaim(new Claim(ClaimTypes.FirstName, user.FirstName));
                    identity.AddClaim(new Claim(ClaimTypes.LastName, user.LastName));
                    identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));

                    // Add Culture claims

                    identity.AddClaim(new Claim(ClaimTypes.CurrencyId, tenant.CurrencyId.ToString()));
                    identity.AddClaim(new Claim(ClaimTypes.TimeZoneId,
                        (user.TimeZoneId ?? Math.Max(1, tenant.TimeZoneId)).ToString()));
                    identity.AddClaim(new Claim(ClaimTypes.LocaleId,
                        (user.LocaleId ?? Math.Max(1, tenant.LocaleId)).ToString()));
                    identity.AddClaim(new Claim(ClaimTypes.LanguageId,
                        (user.LanguageId ?? Math.Max(1, tenant.LanguageId)).ToString()));

                    // Add .NET Localization claims

                    identity.AddClaim(new Claim(ClaimTypes.TimeZoneName,
                        _cache.TimeZones[user.TimeZoneId ?? tenant.TimeZoneId].Name));
                    identity.AddClaim(new Claim(ClaimTypes.LocaleName,
                        _cache.Locales[user.LocaleId ?? tenant.LocaleId].Name));
                }

                return principal;

            }
            catch (Exception ex)
            {

                throw new Exception("In CreateAsync. A claim value is possibly null, this is not allowed.", ex);
            }
        }
    }
}
