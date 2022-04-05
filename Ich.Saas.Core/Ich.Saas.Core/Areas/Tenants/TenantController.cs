using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ich.Saas.Core.Code;
using Ich.Saas.Core.Code.Attributes;
using Ich.Saas.Core.Code.Extensions;
using Ich.Saas.Core.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ich.Saas.Core.Areas.Tenants
{
    [Authorize]
    [Route("tenants")]
    public class TenantController : BaseController
    {
        #region Pages

        [HttpGet]
        public async Task<ViewResult> List(List model)
        {
            await GetAsync(model);
            return View(model);
        }

        [HttpGet("{id}", Order = 10)]
        public async Task<ViewResult> Detail([FromRoute] Detail model)
        {
            await GetAsync(model);
            return View(model);
        }

        [HttpGet("edit/{id?}")]
        public async Task<ViewResult> Edit([FromRoute] int id)
        {
            var model = new Edit {Id = id};
            await GetAsync(model);
            return View(model);
        }

        [HttpPost("edit/{id?}")]
        public async Task<IActionResult> Edit([FromForm] Edit model)
        {
            if (ModelState.IsValid)
            {
                if (await PostAsync(model))
                    return Redirect(model.Referer);
            }

            return View(model);
        }

        [HttpPost("delete"), AjaxOnly]
        public async Task<IActionResult> Delete([FromForm] Delete model)
        {
            await PostAsync(model);
            return Json(true);
        }
        
        #endregion

        #region Handlers

        private async Task GetAsync(List model)
        {
            var tenants = _db.Tenant.AsQueryable();

            // Filter
            switch (model.Filter)
            {
                case 1: 
                    tenants = tenants.Where(t => t.IsActive == true);
                    break;
                case 2:
                    tenants = tenants.Where(t => t.IsActive == false);
                    break;
            }
            
            // Sorting
            tenants = model.Sort switch
            {
                "Name" => tenants.OrderBy(t => t.Name),
                "-Name" => tenants.OrderByDescending(t => t.Name),
                "ContactPerson" => tenants.OrderBy(t => t.ContactPerson),
                "-ContactPerson" => tenants.OrderByDescending(t => t.ContactPerson),
                "ContactEmail" => tenants.OrderBy(t => t.ContactEmail),
                "-ContactEmail" => tenants.OrderByDescending(t => t.ContactEmail),
                "CreatedDate" => tenants.OrderBy(t => t.CreatedDate),
                "-CreatedDate" => tenants.OrderByDescending(t => t.CreatedDate),
                "TotalUsers" => tenants.OrderBy(t => t.TotalUsers),
                "-TotalUsers" => tenants.OrderByDescending(t => t.TotalUsers),
                "IsActive" => tenants.OrderBy(t => t.IsActive),
                "-IsActive" => tenants.OrderByDescending(t => t.IsActive),
                _ => tenants.OrderBy(t => t.Id),
            };
            
            // First get total rows, then paged results
            model.TotalRows = tenants.Count();
            var items = await tenants.Skip(model.Skip).Take(model.Take).ToListAsync();

            _mapper.Map(tenants, model.Items);
        }

        private async Task GetAsync(Detail model)
        {
            var tenant = await SingleTenantAsync(model.Id);
            _mapper.Map(tenant, model);
        }

        private async Task GetAsync(Edit model)
        {
            var tenant = model.Id == 0 ? new Tenant() : await SingleTenantAsync(model.Id);
            _mapper.Map(tenant, model);
        }

        private async Task<bool> PostAsync(Edit model)
        {
            // New tenant
            if (model.Id == 0)
            {
                var tenant = new Tenant();
                _mapper.Map(model, tenant);

                _db.Tenant.Add(tenant);
                await _db.SaveChangesAsync();
                
                // Add countries and departments to new tenant as the application 
                await TenantSetupAsync(tenant);
            }
            else
            {
                var tenant = await SingleTenantAsync(model.Id);
                _mapper.Map(model, tenant);

                _db.Tenant.Update(tenant);
                await _db.SaveChangesAsync();
            }

            return true;
        }

        private async Task PostAsync(Delete model)
        {
            var tenant = await SingleTenantAsync(model.Id);
            _db.Tenant.Remove(tenant);
            await _db.SaveChangesAsync();
        }
        
        #endregion

        #region Helpers

        private async Task<Tenant> SingleTenantAsync(int? id)
        {
            return await _db.Tenant.SingleAsync(t => t.Id == id);
        }

        private async Task TenantSetupAsync(Tenant tenant)
        {
            // Get new tenant started by inserting standard countries and departments

            var dt = DateTime.UtcNow;
            var countries = new List<string>
            {
                "United States", "UK", "France", "Germany", "Australia", "Argentina", "China", "Japan", "Brazil",
                "India", "Egypt", "Denmark", "Mexico", "Sweden"
            };

            foreach (var country in countries)
                _db.Country.Add(new Country
                {
                    TenantId = tenant.Id, Name = country, CreatedOn = dt, ChangedOn = dt
                });
            
            
            var departments = new List<string>
            {
                "Computer Science", "Engineering", "Languages", "Social Sciences", "Natural Sciences"
            };

            foreach (var department in departments)
                _db.Department.Add(new Department { TenantId = tenant.Id, Name = department, CreatedOn = dt, ChangedOn = dt });
            
            await _db.SaveChangesAsync();
        }
        
        #endregion

        #region Mapping

        public class MapperProfile : BaseProfile
        {
            public MapperProfile()
            {
                CreateMap<Tenant, Detail>()
                    .Map(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate.ToDate()))
                    .Map(dest => dest.TimeZone, opt => opt.MapFrom(src => src.TimeZoneId > 0 ? _cache.TimeZones[src.TimeZoneId].DisplayName : null))
                    .Map(dest => dest.Locale, opt => opt.MapFrom(src => src.LocaleId > 0 ? _cache.Locales[src.LocaleId].DisplayName : null))
                    .Map(dest => dest.Language, opt => opt.MapFrom(src => src.LanguageId > 0 ? _cache.Languages[src.LanguageId].Name : null))
                    .Map(dest => dest.Currency, opt => opt.MapFrom(src => src.CurrencyId > 0 ? _cache.Currencies[src.CurrencyId].Name : null));

                CreateMap<Tenant, Edit>()
                    .Map(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate.ToDate()));

                CreateMap<Edit, Tenant>()
                    .Ignore(dest => dest.CreatedDate)
                    .Ignore(dest => dest.TotalUsers);
            }
        }

        #endregion
    }
}