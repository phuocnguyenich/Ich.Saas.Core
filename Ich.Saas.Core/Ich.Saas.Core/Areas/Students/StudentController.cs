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

namespace Ich.Saas.Core.Areas.Students
{
    [Authorize]
    [Menu("Students")]
    [Route("students")]
    public class StudentController : BaseController
    {
        #region Pages

        [HttpGet]
        public async Task<ViewResult> List(List model)
        {
            await GetAsync(model);
            return View(model);
        }

        [HttpGet("{id}", Order = 10)]
        public async Task<ViewResult> Detail(Detail model)
        {
            await GetAsync(model);
            return View(model);
        }

        [HttpGet("edit/{id?}")]
        public async Task<ViewResult> Edit([FromRoute] int id = 0)
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
            var students = _db.Student.Where(s => s.TenantId == _currentTenant.Id).AsQueryable();
            
            // Filters

            if (model.AdvancedFilter)
            {
                if (model.City != null)
                {
                    students = students.Where(s => s.City == model.City);
                }

                if (model.CountryId != null)
                {
                    students = students.Where(s => s.CountryId == model.CountryId);
                }

                if (model.BirthDayFrom != null && DateTime.TryParse(model.BirthDayFrom, out DateTime birthDayFrom))
                {
                    students = students.Where(s => s.BirthDate >= birthDayFrom);
                }

                if (model.BirthDayThru != null && DateTime.TryParse(model.BirthDayThru, out DateTime birthDayThru))
                {
                    students = students.Where(s => s.BirthDate <= birthDayThru);
                }

                if (model.TotalEnrollments != null)
                {
                    students = students.Where(s => s.TotalEnrollments == model.TotalEnrollments);
                }
            }
            else
            {
                switch (model.Filter)
                {
                    case 1: 
                        students = students.Where(s => s.TotalEnrollments > 0);
                        break;
                    case 2:
                        students = students.Where(s => s.TotalEnrollments == 0);
                        break;
                }
            }
            
            // Sorting

            students = model.Sort switch
            {
                "LastName" => students.OrderBy(s => s.LastName),
                "-LastName" => students.OrderByDescending(s => s.LastName),
                "Alias" => students.OrderBy(s => s.Alias),
                "-Alias" => students.OrderByDescending(s => s.Alias),
                "Email" => students.OrderBy(s => s.Email),
                "-Email" => students.OrderByDescending(s => s.Email),
                "City" => students.OrderBy(s => s.City),
                "-City" => students.OrderByDescending(s => s.City),
                "Country" => students.OrderBy(s => s.Country),
                "-Country" => students.OrderByDescending(s => s.Country),
                "BirthDate" => students.OrderBy(s => s.BirthDate),
                "-BirthDate" => students.OrderByDescending(s => s.BirthDate),
                "TotalEnrollments" => students.OrderBy(s => s.TotalEnrollments),
                "-TotalEnrollments" => students.OrderByDescending(s => s.TotalEnrollments),
                _ => students.OrderBy(s => s.Id)
            };

            model.TotalRows = students.Count();
            var items = await students.Skip(model.Skip).Take(model.Take).ToListAsync();

            _mapper.Map(items, model.Items);
        }

        private async Task GetAsync(Detail model)
        {
            var student = await SingleStudentAsync(model.Id);
            await _db.Entry(student).Collection(s => s.Enrollments).LoadAsync();

            _mapper.Map(student, model);
        }
        
        private async Task GetAsync(Edit model)
        {
            var student = model.Id == 0 ? new Student() : await SingleStudentAsync(model.Id);
            _mapper.Map(student, model);
        }

        private async Task<bool> PostAsync(Edit model)
        {
            if (model.Id == 0) // New student
            {
                var student = new Student {IsActive = true};
                _mapper.Map(model, student);

                student.TenantId = _currentTenant.Id.GetValueOrDefault();

                _db.Student.Add(student);
                await _db.SaveChangesAsync();
            }
            else
            {
                var student = await SingleStudentAsync(model.Id);
                _mapper.Map(model, student);

                _db.Student.Update(student);
                await _db.SaveChangesAsync();

                await PostUpdateAsync(student);
            }

            return true;
        }

        private async Task PostAsync(Delete model)
        {
            var student = await SingleStudentAsync(model.Id);
            _db.Student.Remove(student);
            await _db.SaveChangesAsync();
        }
        
        #endregion

        #region Helpers

        private async Task<Student> SingleStudentAsync(int? id)
        {
            return await _db.Student.SingleAsync(s => s.TenantId == _currentTenant.Id && s.Id == id);
        }

        private async Task PostUpdateAsync(Student student)
        {
            await _rollup.RollupStudentAsync(student, _currentTenant.Id);
        }
        #endregion

        #region Mapping

        public class MapperProfile : BaseProfile
        {
            public MapperProfile()
            {
                CreateMap<Student, Detail>()
                    .Map(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate.ToDate()));

                CreateMap<Student, Edit>()
                    .Map(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate.ToDate()));

                CreateMap<Edit, Student>()
                    .Map(dest => dest.Country, opt => opt.MapFrom(src => src.CountryId.HasValue ?_cache.Countries[src.CountryId.Value].Name : null));

                CreateMap<Enrollment, _Enrollment>()
                    .Map(dest => dest.EnrollDate, opt => opt.MapFrom(src => src.EnrollDate.ToDate()))
                    .Map(dest => dest.Fee, opt => opt.MapFrom(src => src.Fee.ToCurrency()))
                    .Map(dest => dest.AmountPaid, opt => opt.MapFrom(src => src.AmountPaid.ToCurrency()));
            }
        }

        #endregion
    }
}