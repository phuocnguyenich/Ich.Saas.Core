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

namespace Ich.Saas.Core.Areas.Enrollments
{
    [Authorize]
    [Menu("Enrollments")]
    [Route("enrollments")]
    public class EnrollmentController : BaseController
    {
        #region Pages

        [HttpGet]
        public async Task<ViewResult> List(List model)
        {
            await GetAsync(model);
            return View(model);
        }

        [HttpGet("{id}")]
        public async Task<ViewResult> Detail([FromRoute]Detail model)
        {
            await GetAsync(model);
            return View(model);
        }

        [HttpGet("edit/{id?}")]
        public async Task<ViewResult> Edit([FromRoute] int id = 0, int? studentId = null, int? classId = null)
        {
            var model = new Edit {Id = id, StudentId = studentId, ClassId = classId};

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
            // Build query with IQueryable<T> interface

            var enrollments = _db.Enrollment.Where(e => e.TenantId == _currentTenant.Id).AsQueryable();

            // Filters

            if (model.AdvancedFilter)
            {
                if (model.StudentId != null)
                {
                    enrollments = enrollments.Where(e => e.StudentId == model.StudentId);
                }

                if (model.ClassId != null)
                {
                    enrollments = enrollments.Where(e => e.ClassId == model.ClassId);
                }

                if (model.Status != null)
                {
                    enrollments = enrollments.Where(e => e.Status == model.Status);
                }

                if (model.AmountPaidFrom != null && decimal.TryParse(model.AmountPaidFrom, out decimal paidFrom))
                {
                    enrollments = enrollments.Where(e => e.AmountPaid >= paidFrom);
                }

                if (model.AmountPaidThru != null && decimal.TryParse(model.AmountPaidThru, out decimal paidThru))
                {
                    enrollments = enrollments.Where(e => e.AmountPaid <= paidThru);
                }
            }
            else
            {
                switch (model.Filter)
                {
                    case 1: enrollments = enrollments.Where(e => e.AmountPaid < e.Fee); break;
                    case 2: enrollments = enrollments.Where(e => e.AmountPaid >= e.Fee); break;
                }
            }

            // Sorting

            enrollments = model.Sort switch
            {
                "EnrollNumber" => enrollments.OrderBy(e => e.EnrollNumber),
                "-EnrollNumber" => enrollments.OrderByDescending(e => e.EnrollNumber),
                "Student" => enrollments.OrderBy(e => e.Student),
                "-Student" => enrollments.OrderByDescending(e => e.Student),
                "Course" => enrollments.OrderBy(e => e.Course),
                "-Course" => enrollments.OrderByDescending(e => e.Course),
                "Class" => enrollments.OrderBy(e => e.Class),
                "-Class" => enrollments.OrderByDescending(e => e.Class),
                "Status" => enrollments.OrderBy(e => e.Status),
                "-Status" => enrollments.OrderByDescending(e => e.Status),
                "Fee" => enrollments.OrderBy(e => e.Fee),
                "-Fee" => enrollments.OrderByDescending(e => e.Fee),
                "AmountPaid" => enrollments.OrderBy(e => e.AmountPaid),
                "-AmountPaid" => enrollments.OrderByDescending(e => e.AmountPaid),
                "TotalQuizzes" => enrollments.OrderBy(e => e.TotalQuizzes),
                "-TotalQuizzes" => enrollments.OrderByDescending(e => e.TotalQuizzes),
                _ => enrollments.OrderBy(e => e.Id),
            };

            // First get total rows, then paged results

            model.TotalRows = enrollments.Count();
            var items = await enrollments.Skip(model.Skip).Take(model.Take).ToListAsync();

            _mapper.Map(items, model.Items);
        }

        private async Task GetAsync(Detail model)
        {
            var enrollment = await SingleEnrollmentAsync(model.Id);
            await _db.Entry(enrollment).Collection(e => e.Quizzes).LoadAsync();

            _mapper.Map(enrollment, model);
        }

        public async Task GetAsync(Edit model)
        {
            var enrollment = model.Id == 0
                ? new Enrollment {EnrollDate = DateTime.UtcNow}
                : await SingleEnrollmentAsync(model.Id);

            _mapper.Map(enrollment, model);
        }

        private async Task<bool> PostAsync(Edit model)
        {
            if (model.Id == 0) // new enrollment
            {
                // student can enroll only once for each class

                int enrollments = _db.Enrollment.Count(e => e.StudentId == model.StudentId
                                                            && e.ClassId == model.ClassId
                                                            && e.TenantId == _currentTenant.Id);

                if (enrollments > 0)
                {
                    ModelState.AddModelError("StudentId", _localizer["Student is already enrolled in this class"]);
                    return false;
                }

                try
                {
                    var enrollment = new Enrollment();
                    _mapper.Map(model, enrollment);

                    if (_currentTenant.Id != null) enrollment.TenantId = _currentTenant.Id.Value;
                    
                    // ** Unit of work

                    using (var transaction = await _db.Database.BeginTransactionAsync())
                    {
                        _db.Enrollment.Add(enrollment);
                        await _db.SaveChangesAsync();

                        enrollment.EnrollNumber = string.Format("{0:ENR-00000}", enrollment);
                        _db.Enrollment.Update(enrollment);
                        await _db.SaveChangesAsync();

                        await transaction.CommitAsync();
                    }

                    await PostInsertAsync(enrollment);
                }
                catch (Exception ex)
                {
                    var message = ex.Message;
                    Failure = "Insert failed. Please try again";
                    return false;
                }
            }
            else
            {
                var enrollment = await SingleEnrollmentAsync(model.Id);

                _mapper.Map(model, enrollment);

                _db.Enrollment.Update(enrollment);
                await _db.SaveChangesAsync();

                await PostUpdateAsync(enrollment);
            }

            return true;
        }

        private async Task PostAsync(Delete model)
        {
            var enrollment = await SingleEnrollmentAsync(model.Id);
            _db.Enrollment.Remove(enrollment);
            await _db.SaveChangesAsync();

            await PostDeleteAsync(enrollment);
        }
        #endregion

        #region Helpers

        private async Task<Enrollment> SingleEnrollmentAsync(int? id)
        {
            return await _db.Enrollment.SingleAsync(c => c.TenantId == _currentTenant.Id && c.Id == id);
        }

        private async Task PostInsertAsync(Enrollment enrollment)
        {
            await _rollup.RollupEnrollmentAsync(enrollment, _currentTenant.Id);
        }

        private async Task PostUpdateAsync(Enrollment enrollment)
        {
            await _rollup.RollupEnrollmentAsync(enrollment, _currentTenant.Id);
        }

        private async Task PostDeleteAsync(Enrollment enrollment)
        {
            await _rollup.RollupEnrollmentAsync(enrollment, _currentTenant.Id);
        }
        
        #endregion

        #region Mapping

        public class MapperProfile : BaseProfile
        {
            public MapperProfile()
            {
                CreateMap<Enrollment, Detail>()
                    .Map(dest => dest.EnrollDate, opt => opt.MapFrom(src => src.EnrollDate.ToDate()))
                    .Map(dest => dest.Fee, opt => opt.MapFrom(src => src.Fee.ToCurrency()))
                    .Map(dest => dest.AmountPaid, opt => opt.MapFrom(src => src.AmountPaid.ToCurrency()))
                    .Map(dest => dest.Status, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.Status) ? "" : _localizer[src.Status]));

                CreateMap<Enrollment, Edit>()
                    .Map(dest => dest.EnrollDate, opt => opt.MapFrom(src => src.EnrollDate.ToDate()));

                CreateMap<Edit, Enrollment>()
                    .Map(dest => dest.CourseId, opt => opt.MapFrom(src => _db.Class.Single(c => c.TenantId == _currentTenant.Id && c.Id == src.ClassId).CourseId))
                    .Ignore(dest => dest.EnrollNumber);

                CreateMap<Quiz, _Quiz>()
                    .Map(dest => dest.QuizDate, opt => opt.MapFrom(src => src.QuizDate.ToDate()))
                    .Map(dest => dest.Grade, opt => opt.MapFrom(src => src.Grade.ToString("0.##")));
            }
        }

        #endregion
    }
}