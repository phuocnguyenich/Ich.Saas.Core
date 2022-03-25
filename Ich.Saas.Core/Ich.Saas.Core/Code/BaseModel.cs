using Ich.Saas.Core.Code.Extensions;
using Ich.Saas.Core.Code.Infrastructure;
using Microsoft.AspNetCore.Http;

namespace Ich.Saas.Core.Code
{
    public class BaseModel
    {
        private IHttpContextAccessor _httpContextAccessor = ServiceLocator.Resolve<IHttpContextAccessor>();

        public string Referer { get => _httpContextAccessor.PostedReferer() ?? 
                                       _httpContextAccessor.Referer(); }
    }
}