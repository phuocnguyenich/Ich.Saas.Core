using System.Threading.Tasks;
using Ich.Saas.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ich.Saas.Core.Code.Database
{
    #region Interface

    public interface IRollup
    {
        Task RollupStudentAsync(Student student, int? tenantId);
        Task RollupEnrollmentAsync(Enrollment enrollment, int? tenantId);
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

        public async Task RollupEnrollmentAsync(Enrollment enrollment, int? tenantId)
        {
            await _db.Database.ExecuteSqlInterpolatedAsync(
                @$"UPDATE [Class] 
                          SET TotalEnrollments = (SELECT COUNT(E.Id) 
                                                  FROM [Enrollment] E 
                                                  WHERE E.ClassId = K.Id 
                                                  AND E.TenantId = K.TenantId
                                                  AND E.TenantId = {tenantId})
                        FROM [Class] K
                        WHERE K.TenantId = {tenantId} AND K.Id = {enrollment.ClassId};");
            
            await _db.Database.ExecuteSqlInterpolatedAsync(
                $@"UPDATE [Student] 
                          SET TotalEnrollments = (SELECT COUNT(E.Id) 
                                                  FROM [Enrollment] E 
                                                  WHERE E.StudentId = S.Id 
                                                  AND E.TenantId = S.TenantId
                                                  AND E.TenantId = {tenantId})
                        FROM [Student] S
                        WHERE S.TenantId = {tenantId} AND S.Id = {enrollment.StudentId};");

            await _db.Database.ExecuteSqlInterpolatedAsync(
                $@"UPDATE [Quiz] 
                          SET [Quiz].Enrollment = E.EnrollNumber
                         FROM [Quiz] Q
                         JOIN [Enrollment] E ON (Q.EnrollmentId = Q.Id AND Q.TenantId = E.TenantId)
                        WHERE Q.TenantId = {tenantId} AND Q.EnrollmentId = {enrollment.Id}; ");
            
            await _db.Database.ExecuteSqlInterpolatedAsync(
                $@"UPDATE [Enrollment] 
                          SET TotalQuizzes = (SELECT COUNT(Q.Id) 
                                              FROM [Quiz] Q 
                                              WHERE Q.EnrollmentId = E.Id  
                                              AND Q.TenantId = E.TenantId),
                              AverageGrade = (SELECT AVG(Q.Grade) 
                                              FROM [Quiz] Q 
                                              WHERE Q.EnrollmentId = E.Id  
                                              AND Q.TenantId = E.TenantId),
                              [Enrollment].Student = S.FirstName + ' ' + S.LastName,
                              [Enrollment].Course = C.Title,
                              [Enrollment].Fee = C.Fee,
                              [Enrollment].Class = K.Location 
                        FROM [Enrollment] E
                        JOIN [Student] S ON (E.StudentId = S.Id AND E.TenantId = S.TenantId)
                        JOIN [Course] C ON (E.CourseId = C.Id AND E.TenantId = C.TenantId)
                        JOIN [Class] K ON (E.ClassId =  K.Id AND E.TenantId = K.TenantId)
                        WHERE E.TenantId = {tenantId} AND E.Id = {enrollment.Id}");
        }
    }

    #endregion
}