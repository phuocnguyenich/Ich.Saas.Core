using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ich.Saas.Core.Code;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ich.Saas.Core.Areas.Auth
{
    public class AuthController : BaseController
    {
        #region Logout

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await _identityService.SignOutAsync();
            return RedirectToAction("Login");
        }

        #endregion

        #region Handlers

        private async Task GetAsync(Login model)
        {
            var list = new List<_User>();

            // Hardcode set of users for fast login. Don't do this in your own code :))

            var users = _db.User.Where(u => new int?[] {1, 2, 3, 4, 5}.Contains(u.Id));

            var tenants = await _db.Tenant.ToDictionaryAsync(t => t.Id);

            foreach (var user in users.OrderBy(u => u.Id == 1 ? 10 : u.Id))
            {
                var tenant = tenants[user.TenantId];

                var _user = new _User
                {
                    Tenant = tenant.Name,
                    UserId = user.Id,
                    User = user.FullName,
                    Role = user.Role,
                    Color = tenant.Color,
                    Email = user.Email,
                    Password = "Secret123!",
                    TimeZone = _cache.TimeZones[user.TimeZoneId ?? tenant.TimeZoneId].Name,
                    Language = _cache.Languages[user.LanguageId ?? tenant.LanguageId].Name,
                    Locale = _cache.Locales[user.LocaleId ?? tenant.LocaleId].Name,
                    Currency = _cache.Currencies[tenant.CurrencyId].Name
                };

                list.Add(_user);
            }

            model.Users = list;
        }

        #endregion

        #region Login

        [HttpGet("login")]
        public async Task<ViewResult> Login(string returnUrl)
        {
            var model = new Login {ReturnUrl = returnUrl};
            await GetAsync(model);
            return View(model);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] Login model)
        {
            if (ModelState.IsValid)
            {
                var result = await _identityService.PasswordSignInAsync(model.Email, model.Password);

                if (result.Succeeded)
                {
                    var user = _db.User.SingleOrDefault(u => u.Email == model.Email);

                    var url = user is {Role: "Host"} ? "/tenants" : "/students";

                    if (user != null && Url.IsLocalUrl(model.ReturnUrl) && user.Role != "Host")
                        url = model.ReturnUrl;

                    return Redirect(url);
                }
            }

            Failure = _localizer["Login was unsuccessfully"];

            return View(model);
        }

        #endregion
    }
}