using System;
using System.Linq;
using System.Threading.Tasks;
using Ich.Saas.Core.Code;
using Ich.Saas.Core.Code.Attributes;
using Ich.Saas.Core.Code.Extensions;
using Ich.Saas.Core.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ich.Saas.Core.Areas.Classes
{
    [Authorize]
    [Menu("Classes")]
    [Route("classes")]
    public class ClassController : BaseController
    {
        #region Pages

        [HttpGet]
        public async Task<ViewResult> List(List model)
        {
            await GetAsync(model);
            return View(model);
        }

        [HttpGet("{id}")]
        public async Task<ViewResult> Detail(Detail model)
        {
            await GetAsync(model);
            return View(model);
        }

        [HttpGet("edit/{id?}")]
        public async Task<ViewResult> Edit([FromRoute]int id = 0, int? courseId = null)
        {
            var model = new Edit { Id = id, CourseId = courseId };
            await GetAsync(model);
            return View(model);
        }

        [HttpPost("edit/{id?}")]
        public async Task<IActionResult> Edit([FromForm]Edit model)
        {
            if (ModelState.IsValid)
            {
                if (await PostAsync(model))
                    return Redirect(model.Referer);
            }

            return View(model);
        }

        [AjaxOnly]
        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromForm]Delete model)
        {
            await PostAsync(model);
            return Json(true);
        }

        #endregion

        #region Handlers

        private async Task GetAsync(List model)
        {
            var classes = _db.Class.Where(c => c.TenantId == _currentTenant.Id).AsQueryable();

            // Filters

            if (model.AdvancedFilter)
            {
                if (model.CourseId != null)
                {
                    classes = classes.Where(c => c.CourseId == int.Parse(model.CourseId));
                }

                if (model.Location != null)
                {
                    classes = classes.Where(c => c.Location == model.Location);
                }

                if (model.TotalEnrollmentsFrom != null && int.TryParse(model.TotalEnrollmentsFrom, out int totalFrom))
                {
                    classes = classes.Where(c => c.TotalEnrollments >= totalFrom);
                }


                if (model.TotalEnrollmentsThru != null && int.TryParse(model.TotalEnrollmentsThru, out int totalThru))
                {
                    classes = classes.Where(c => c.TotalEnrollments <= totalThru);
                }

                if (model.StartDateFrom != null && DateTime.TryParse(model.StartDateFrom, out DateTime dateFrom))
                {
                    classes = classes.Where(c => c.StartDate >= dateFrom);
                }

                if (model.StartDateThru != null && DateTime.TryParse(model.StartDateThru, out DateTime dateThru))
                {
                    classes = classes.Where(c => c.StartDate <= dateThru);
                }
            }
            else
            {
                switch (model.Filter)
                {
                    case 1: classes = classes.Where(c => c.MaxEnrollments > 15); break;
                    case 2: classes = classes.Where(c => c.TotalEnrollments > 2); break;
                }
            }

            // Sorting

            classes = model.Sort switch
            {
                "ClassNumber" => classes.OrderBy(c => c.ClassNumber),
                "-ClassNumber" => classes.OrderByDescending(c => c.ClassNumber),
                "Course" => classes.OrderBy(c => c.Course),
                "-Course" => classes.OrderByDescending(c => c.Course),
                "StartDate" => classes.OrderBy(c => c.StartDate),
                "-StartDate" => classes.OrderByDescending(c => c.StartDate),
                "EndDate" => classes.OrderBy(c => c.EndDate),
                "-EndDate" => classes.OrderByDescending(c => c.EndDate),
                "Location" => classes.OrderBy(c => c.Location),
                "-Location" => classes.OrderByDescending(c => c.Location),
                "MaxEnrollments" => classes.OrderBy(c => c.MaxEnrollments),
                "-MaxEnrollments" => classes.OrderByDescending(c => c.MaxEnrollments),
                "TotalEnrollments" => classes.OrderBy(c => c.TotalEnrollments),
                "-TotalEnrollments" => classes.OrderByDescending(c => c.TotalEnrollments),
                _ => classes.OrderBy(c => c.Id),
            };

            // First get total rows, then paged results

            model.TotalRows = classes.Count();
            var items = await classes.Skip(model.Skip).Take(model.Take).ToListAsync();

            _mapper.Map(items, model.Items);
        }

        private async Task GetAsync(Detail model)
        {
            var cls = await SingleClassAsync(model.Id);
            await _db.Entry(cls).Collection(c => c.Enrollments).LoadAsync(); // top 9

            _mapper.Map(cls, model);
        }

        private async Task GetAsync(Edit model)
        {
            if (model.Id > 0)
            {
                var cls = await SingleClassAsync(model.Id);
                _mapper.Map(cls, model);
            }
        }

        private async Task<bool> PostAsync(Edit model)
        {
            if (model.Id == 0) // new class
            {
                try
                {
                    var cls = new Class();
                    _mapper.Map(model, cls);

                    if (_currentTenant.Id != null) cls.TenantId = _currentTenant.Id.Value;

                    // ** Unit of work pattern

                    using (var transaction = await _db.Database.BeginTransactionAsync())
                    {

                        _db.Class.Add(cls);
                        await _db.SaveChangesAsync();

                        cls.ClassNumber = string.Format("{0:CLS-00000}", cls.Id);

                        _db.Class.Update(cls);
                        await _db.SaveChangesAsync();

                        await transaction.CommitAsync();
                    }

                    await PostInsertAsync(cls);
                }
                catch
                {
                    Failure = "Save failed. Please try again.";
                    return false;
                }
            }
            else
            {
                var cls = await SingleClassAsync(model.Id);
                _mapper.Map(model, cls);
                
                _db.Class.Update(cls);
                await _db.SaveChangesAsync();

                await PostUpdateAsync(cls);
            }

            return true;
        }

        private async Task PostAsync(Delete model)
        {
            var cls = await SingleClassAsync(model.Id);
            _db.Class.Remove(cls);
            await _db.SaveChangesAsync();

            await PostDeleteAsync(cls);
        }

        #endregion

        #region Helpers

        private async Task<Class> SingleClassAsync(int? id)
        {
            return await _db.Class.SingleAsync(c => c.TenantId == _currentTenant.Id && c.Id == id);
        }

        private async Task PostInsertAsync(Class cls)
        {
            await _rollup.RollupClassAsync(cls, _currentTenant.Id);
        }

        private async Task PostUpdateAsync(Class cls)
        {
            await _rollup.RollupClassAsync(cls, _currentTenant.Id);
        }

        private async Task PostDeleteAsync(Class cls)
        {
            await _rollup.RollupClassAsync(cls, _currentTenant.Id);
        }

        #endregion

        #region Mapping

        public class MapperProfile : BaseProfile
        {
            public MapperProfile()
            {
                CreateMap<Class, Detail>()
                   .Map(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate.ToDate()))
                   .Map(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate.ToDate()));

                CreateMap<Class, Edit>()
                   .Map(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate.ToDate()))
                   .Map(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate.ToDate()));

                CreateMap<Edit, Class>()
                   .Map(dest => dest.Course, opt => opt.MapFrom(src => src.CourseId != null ? _db.Course.Single(c => c.Id == src.CourseId).Title : null))
                   .Ignore(dest => dest.ClassNumber);

                CreateMap<Enrollment, _Enrollment>()
                   .Map(dest => dest.EnrollDate, opt => opt.MapFrom(src => src.EnrollDate.ToDate()))
                   .Map(dest => dest.Fee, opt => opt.MapFrom(src => src.Fee.ToCurrency()))
                   .Map(dest => dest.AmountPaid, opt => opt.MapFrom(src => src.AmountPaid.ToCurrency()));
            }
        }

        #endregion
    }
}