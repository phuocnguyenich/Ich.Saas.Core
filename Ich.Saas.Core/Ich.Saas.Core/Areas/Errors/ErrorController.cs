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
using Microsoft.Extensions.Logging;

namespace Ich.Saas.Core.Areas.Errors
{
    [Authorize(Roles = "Host")]
    [Menu("Errors")]
    [Route("errors")]
    public class ErrorController : BaseController
    {
        #region Pages

        [HttpGet]
        public async Task<ViewResult> List(List model)
        {
            await GetAsync(model);
            return View(model);
        }

        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromForm] Delete model)
        {
            if (ModelState.IsValid)
            {
                if (await PostAsync(model))
                    return RedirectToAction("List");
            }

            return View(model);
        }

        #endregion

        #region Handlers

        private async Task GetAsync(List model)
        {
            model.TotalRows = await _db.Error.CountAsync();

            var errors = _db.Error
                .Include(e => e.User)
                .Include(e => e.Tenant)
                .OrderByDescending(e => e.CreatedDate)
                .Skip(model.Skip).Take(model.Take);

            _mapper.Map(errors, model.Items);
        }

        
        private async Task<bool> PostAsync(Delete model)
        {
            try
            {
                if (model.DeleteCount == "All")
                {
                    await _db.Database.ExecuteSqlRawAsync("TRUNCATE TABLE [Error]");
                    Success = "All error records deleted";
                }
                else
                {
                    int totalCount = await _db.Error.CountAsync();
                    int deleteCount = int.Parse(model.DeleteCount);
                    string sql =
                        $"DELETE FROM [Error] WHERE Id IN (SELECT TOP {deleteCount} Id FROM [Error] ORDER BY Id)";
                    await _db.Database.ExecuteSqlRawAsync(sql);

                    Success = Math.Min(totalCount, deleteCount) + " error records deleted";
                }
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger<ErrorController>();
                logger.LogError(0, ex, "Error deleting error records");

                Failure = "Unable to delete error records";

                return false;
            }

            return true;
        }
        
        #endregion

        #region Mapping

        public class MapperProfile : BaseProfile
        {
            public MapperProfile()
            {
                CreateMap<Error, Detail>()
                    .Map(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate.ToDate()));
            }
        }

        #endregion
    }
}