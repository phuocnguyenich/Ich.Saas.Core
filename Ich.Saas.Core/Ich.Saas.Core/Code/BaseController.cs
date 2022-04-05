using AutoMapper;
using Ich.Saas.Core.Code.Caching;
using Ich.Saas.Core.Code.Identity;
using Ich.Saas.Core.Code.Localization;
using Ich.Saas.Core.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Ich.Saas.Core.Code
{
    public class BaseController : Controller
    {
        #region Dependency Injection

        // ** Lazy Injection pattern

        private ICache cache;
        private IMapper mapper;
        private SaaSContext db;
        private ICurrentUser currentUser;
        private ICurrentTenant currentTenant;
        private ILoggerFactory loggerFactory;
        private IIdentityService identityService;
        private IStringLocalizer<SharedResources> localizer;

        protected ICache _cache => cache ??= HttpContext.RequestServices.GetService<ICache>();
        protected IMapper _mapper => mapper ??= HttpContext.RequestServices.GetService<IMapper>();
        protected SaaSContext _db => db ??= HttpContext.RequestServices.GetService<SaaSContext>();
        protected ICurrentUser _currentUser => currentUser ??= HttpContext.RequestServices.GetService<ICurrentUser>();
        protected ICurrentTenant _currentTenant => currentTenant ??= HttpContext.RequestServices.GetService<ICurrentTenant>();
        protected ILoggerFactory _loggerFactory => loggerFactory ??= HttpContext.RequestServices.GetService<ILoggerFactory>();
        protected IIdentityService _identityService => identityService ??= HttpContext.RequestServices.GetService<IIdentityService>();
        protected IStringLocalizer<SharedResources> _localizer => localizer ??= HttpContext.RequestServices.GetService<IStringLocalizer<SharedResources>>();
        
        #endregion
        
        #region Meta

        public string Title { set => ViewBag.Title = value; }
        public string Keywords { set => ViewBag.Keywords = value; }
        public string Description { set => ViewBag.Description = value; }

        #endregion

        #region Alerts

        // Success and Failure contain alert messages that are available even following a redirect.

        public string Success { set => TempData["Success"] = value; get => TempData["Success"]?.ToString(); }
        public string Failure { set => TempData["Failure"] = value; get => TempData["Failure"]?.ToString(); }

        #endregion
    }
}