namespace Ich.Saas.Core.Areas.Classes
{
    public class _Enrollment  
    {
        public int? Id { get; set; }
        public string EnrollNumber { get; set; }
        public int? CourseId { get; set; }
        public int? StudentId { get; set; }
        public string Student { get; set; }
        public string EnrollDate { get; set; }
        public string AmountPaid { get; set; }
        public string Status { get; set; }
        public string Fee { get; set; }
        public int TotalQuizzes { get; set; }
    }
}