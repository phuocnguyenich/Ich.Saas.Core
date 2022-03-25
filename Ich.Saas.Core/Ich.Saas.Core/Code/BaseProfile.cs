using AutoMapper;
using Ich.Saas.Core.Code.Caching;
using Ich.Saas.Core.Code.Identity;
using Ich.Saas.Core.Code.Infrastructure;
using Ich.Saas.Core.Code.Localization;
using Ich.Saas.Core.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace Ich.Saas.Core.Code
{
    public class BaseProfile : Profile
    {
        // Base class to all AutoMapper Profiles

        #region Dependency Injection

        // ** Lazy Injection pattern

        private static HttpContext HttpContext => ServiceLocator.Resolve<IHttpContextAccessor>().HttpContext;
        
        private ICurrentUser currentUser;
        private ICurrentTenant currentTenant;
        private IStringLocalizer<SharedResources> localizer;

        // Lifetime = Scoped
        
        protected ICache _cache => HttpContext.RequestServices.GetService<ICache>();
        protected SaaSContext _db => HttpContext.RequestServices.GetService<SaaSContext>();
        
        // Lifetime = Singleton
        
        protected ICurrentUser _currentUser => currentUser ??= HttpContext.RequestServices.GetService<ICurrentUser>();
        protected ICurrentTenant _currentTenant => currentTenant ??= HttpContext.RequestServices.GetService<ICurrentTenant>();
        protected IStringLocalizer<SharedResources> _localizer => localizer ??= HttpContext.RequestServices.GetService<IStringLocalizer<SharedResources>>();

        #endregion
    }
}