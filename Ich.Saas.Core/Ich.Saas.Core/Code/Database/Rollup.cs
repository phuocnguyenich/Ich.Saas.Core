using System.Threading.Tasks;
using Ich.Saas.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ich.Saas.Core.Code.Database
{
    #region Interface

    public interface IRollup
    {
        Task RollupStudentAsync(Student student, int? tenantId);
    }

    #endregion

    #region Implementation

    public class Rollup : IRollup
    {
        #region Dependency Injection

        private readonly SaaSContext _db;

        public Rollup(SaaSContext db)
        {
            _db = db;
        }

        #endregion
        
        public async Task RollupStudentAsync(Student student, int? tenantId)
        {
            await _db.Database.ExecuteSqlInterpolatedAsync(
                $@"UPDATE [Enrollment] 
                         SET [Enrollment].Student = S.FirstName + ' ' + S.LastName
                         FROM [Enrollment] E
                         JOIN [Student] S ON (E.StudentId = S.Id AND E.TenantId = S.TenantId)
                        WHERE E.TenantId = {tenantId} AND E.StudentId = {student.Id};");
        }
    }

    #endregion
}