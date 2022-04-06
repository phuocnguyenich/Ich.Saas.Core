namespace Ich.Saas.Core.Areas.Students
{
    public class _Enrollment
    {
        public int Id { get; set; }
        public string EnrollNumber { get; set; }
        public string EnrollDate { get; set; }
        public string Course { get; set; }
        public string Fee { get; set; }
        public string AmountPaid { get; set; }
        public decimal? AverageGrade { get; set; }
        public int TotalQuizzes { get; set; }
    }
}