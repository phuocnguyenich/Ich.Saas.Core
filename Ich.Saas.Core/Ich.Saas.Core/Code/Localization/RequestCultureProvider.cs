using System.Threading.Tasks;
using Ich.Saas.Core.Code.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;

namespace Ich.Saas.Core.Code.Localization
{
    public class RequestCultureProvider : IRequestCultureProvider
    {
        public Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            var currentUser = httpContext.RequestServices.GetService<ICurrentUser>() as CurrentUser;

            var culture = currentUser?.CultureInfo?.Name ?? "en-US";
            var uiCulture = currentUser?.CultureInfo?.Name ?? "en-US";

            var result = new ProviderCultureResult(culture, uiCulture);
            return Task.FromResult(result);
        }
    }
}