using System.Threading.Tasks;
using Ich.Saas.Core.Code;
using Ich.Saas.Core.Code.Extensions;
using Ich.Saas.Core.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ich.Saas.Core.Areas.Accounts
{
    [Authorize]
    [Route("account")]
    public class AccountController : BaseController
    {
        #region Pages

        [HttpGet]
        public async Task<ViewResult> Detail(Detail model)
        {
            await GetAsync(model);
            return View(model);
        }

        [HttpGet("edit")]
        public async Task<ViewResult> Edit()
        {
            var model = new Edit();
            await GetAsync(model);
            return View(model);
        }

        [HttpGet("password")]
        public ViewResult Password()
        {
            var model = new Password();
            return View(model);
        }

        [HttpPost("edit")]
        public async Task<IActionResult> Edit([FromForm] Edit model)
        {
            if (ModelState.IsValid)
            {
                await PostAsync(model);
                return RedirectToAction("Detail");
            }

            return View(model);
        }

        [HttpPost("password")]
        public async Task<IActionResult> Password([FromForm] Password model)
        {
            if (ModelState.IsValid)
            {
                var user = await SingleUserAsync();

                var result = await _identityService.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

                if (result.Succeeded)
                {
                    Success = _localizer["Password has been changed"];
                    return RedirectToAction("Detail");
                }

                Failure = _localizer["Password change failed"];
            }

            return View(model);
        }

        #endregion

        #region Handlers

        private async Task GetAsync(Detail model)
        {
            var user = await SingleUserAsync();
            _mapper.Map(user, model);
        }
        
        private async Task GetAsync(Edit model)
        {
            var user = await SingleUserAsync();
            _mapper.Map(user, model);
        }
        
        
        private async Task PostAsync(Edit model)
        {
            var user = await SingleUserAsync();
            _mapper.Map(model, user);
            
            _db.User.Update(user);
            await _db.SaveChangesAsync();
            
            // Refresh claim as culture settings may have changed
            await _identityService.RefreshClaimsAsync(user);
        }
        #endregion

        #region Helpers

        private async Task<User> SingleUserAsync()
        {
            return await _db.User.SingleAsync(u => u.TenantId == _currentTenant.Id && u.Id == _currentUser.Id);
        }

        #endregion

        #region Mapping

        public class MapperProfile : BaseProfile
        {
            public MapperProfile()
            {
                CreateMap<User, Detail>()
                    .Map(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate.ToDate()))
                    .Map(dest => dest.TimeZone, opt => opt.MapFrom(src => src.TimeZoneId != null ? _cache.TimeZones[src.TimeZoneId.Value].DisplayName : null))
                    .Map(dest => dest.Locale, opt => opt.MapFrom(src => src.LocaleId != null ? _cache.Locales[src.LocaleId.Value].DisplayName : null))
                    .Map(dest => dest.Language, opt => opt.MapFrom(src => src.LanguageId != null ? _cache.Languages[src.LanguageId.Value].Name : null));

                CreateMap<User, Edit>();
                CreateMap<Edit, User>();
            }
        }

        #endregion
    }
}