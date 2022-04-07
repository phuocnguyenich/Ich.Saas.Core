using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ich.Saas.Core.Code;

namespace Ich.Saas.Core.Areas.Enrollments
{
    public class Detail : BaseModel
    {
        public int Id { get; set; }
        public int? TenantId { get; set; }
        public string EnrollNumber { get; set; }
        public int? StudentId { get; set; }
        public string Student { get; set; }
        public int? ClassId { get; set; }
        public string Class { get; set; }
        public int? CourseId { get; set; }
        public string Course { get; set; }
        public string EnrollDate { get; set; }
        public string AmountPaid { get; set; }
        public string Status { get; set; }
        public decimal? AverageGrade { get; set; }
        public string Fee { get; set; }
        public int TotalQuizzes { get; set; }

        public int? QuizId { get; set; }

        // Related list

        public string Tab { get; set; } = "details";

        public List<_Quiz> Quizzes { get; set; } = new List<_Quiz>();

        // Helper fields for adding quizzes

        [Required(ErrorMessage = "Quizdate is required")]
        public string QuizDate { get; set; }
        [Required(ErrorMessage = "Grade is required")]
        public decimal Grade { get; set; }

    }
}