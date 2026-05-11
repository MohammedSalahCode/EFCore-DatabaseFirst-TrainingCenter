using Microsoft.EntityFrameworkCore;
using TrainingCenter.Data;

namespace TrainingCenter.Services
{
    public class StatisticsService
    {
        private readonly AppDbContext _context;

        public StatisticsService(AppDbContext context)
        {
            _context = context;
        }


        // =========================
        // Existence Checks
        // =========================

        public void CheckActiveStudentsExist()
        {
            bool exists = _context.Students
                .Any(s => s.Status == "Active");

            Console.WriteLine($"Has Active Students: {exists}");
        }


        public void CheckAllCoursesHaveValidPrice()
        {
            bool allValid = _context.Courses.All(c => c.Price > 0);

            Console.WriteLine($"All Courses Have Valid Price: {allValid}");
        }


        // =========================
        // Aggregations
        // =========================

        public void GetActiveStudentsCount()
        {
            int count = _context.Students
                .Where(s => s.Status == "Active")
                .Count();

            Console.WriteLine($"Active Students Count: {count}");
        }


        public void GetAverageEnrollmentProgress()
        {
            decimal average = _context.Enrollments
                .Average(e => e.ProgressPercent);

            Console.WriteLine($"Average Progress: {average}");
        }


        public void GetTotalCoursesDuration()
        {
            int totalHours = _context.Courses
                .Sum(c => c.DurationHours);

            Console.WriteLine($"Total Course Hours: {totalHours}");
        }


        // =========================
        // Min / Max Aggregations
        // =========================

        public void GetMinAndMaxCoursePrices()
        {
            decimal minPrice = _context.Courses.Min(c => c.Price);
            decimal maxPrice = _context.Courses.Max(c => c.Price);

            Console.WriteLine($"Lowest Course Price : {minPrice}");
            Console.WriteLine($"Highest Course Price: {maxPrice}");
        }


        // =========================
        // Reporting Queries
        // =========================

        public void GetDistinctStudentStatuses()
        {
            var statuses = _context.Students
                .Select(s => s.Status)
                .Distinct()
                .ToList();

            foreach (var status in statuses)
            {
                Console.WriteLine(status);
            }
        }


        public void GetStudentsCountPerStatus()
        {
            var report = _context.Students
                .GroupBy(s => s.Status)
                .Select(group => new
                {
                    Status = group.Key,
                    TotalStudents = group.Count()
                })
                .OrderBy(x => x.Status)
                .ToList();

            foreach (var row in report)
            {
                Console.WriteLine($"{row.Status} : {row.TotalStudents}");
            }
        }


        public void GetStatusesHavingMoreThanTenStudents()
        {
            var report = _context.Students
                .GroupBy(s => s.Status)
                .Where(g => g.Count() > 10)
                .Select(group => new
                {
                    Status = group.Key,
                    TotalStudents = group.Count()
                })
                .OrderBy(x => x.Status)
                .ToList();

            foreach (var row in report)
            {
                Console.WriteLine($"{row.Status} : {row.TotalStudents}");
            }
        }
    }
}