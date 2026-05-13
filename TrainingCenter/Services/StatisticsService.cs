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


        // =========================
        // Related Data Loading
        // =========================

        public void DemonstrateNPlusOneProblem()
        {
            var students = _context.Students.ToList();

            foreach (var student in students)
            {
                int enrollmentsCount = _context.Enrollments
                    .Count(e => e.StudentId == student.StudentId);

                Console.WriteLine(
                    $"{student.FirstName} {student.LastName} - Enrollments: {enrollmentsCount}");
            }
        }


        public void GetStudentsWithEnrollmentsUsingInclude()
        {
            var students = _context.Students
                .Include(s => s.Enrollments)
                .ToList();

            foreach (var student in students)
            {
                Console.WriteLine(
                    $"{student.FirstName} {student.LastName} - Enrollments: {student.Enrollments.Count}");
            }
        }

        public void GetStudentsWithEnrollmentCountUsingProjection()
        {
            var students = _context.Students
                .Select(s => new
                {
                    FullName = s.FirstName + " " + s.LastName,
                    EnrollmentsCount = s.Enrollments.Count()
                })
                .ToList();

            foreach (var student in students)
            {
                Console.WriteLine(
                    $"{student.FullName} - Enrollments: {student.EnrollmentsCount}");
            }
        }


        public void GetStudentsWithEnrollmentsAndCourses()
        {
            var students = _context.Students
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                .OrderBy(s => s.StudentId)
                .ToList();

            foreach (var student in students)
            {
                Console.WriteLine(
                    $"{student.StudentId} - {student.FirstName} {student.LastName}");


                foreach (var enrollment in student.Enrollments)
                {
                    Console.WriteLine(
                        $"Course: {enrollment.Course.Title}, " +
                        $"Status: {enrollment.Status}, " +
                        $"Progress: {enrollment.ProgressPercent}%");
                }

                Console.WriteLine();
            }
        }


        public void GetStudentsSummaryReport()
        {
            var report = _context.Students
                .Select(s => new
                {
                    FullName = s.FirstName + " " + s.LastName,
                    CourseCount = s.Enrollments.Count(),
                    TotalPaid = s.Enrollments.Sum(e => e.Course.Price)
                })
                .ToList();

            foreach (var item in report)
            {
                Console.WriteLine($"{item.FullName} - {item.CourseCount} - {item.TotalPaid}");
            }
        }




        public void GetCoursesWithInstructorsUsingJoin()
        {
            var report = _context.Courses
                .Join(
                    _context.Instructors,
                    course => course.InstructorId,
                    instructor => instructor.InstructorId,
                    (course, instructor) => new
                    {
                        course.Title,
                        course.Code,
                        InstructorName =
                            instructor.FirstName + " " + instructor.LastName
                    })
                .OrderBy(x => x.Title)
                .ToList();

            foreach (var item in report)
            {
                Console.WriteLine(
                    $"{item.Code} - {item.Title} - {item.InstructorName}");
            }
        }


        public void GetStudentsWithProfilesUsingLeftJoin()
        {
            var report =

                from student in _context.Students
                join profile in _context.StudentProfiles
                    on student.StudentId equals profile.StudentId
                    into profilesGroup
                from subProfile in profilesGroup.DefaultIfEmpty()

                select new
                {
                    student.StudentId,
                    FullName = student.FirstName + " " + student.LastName,
                    City = subProfile != null ? subProfile.City : "No City Provided",
                    Country = subProfile != null ? subProfile.Country : "No Country Provided"
                };

            var results = report.OrderBy(s => s.StudentId).ToList();

            foreach (var row in results)
            {
                Console.WriteLine($"{row.StudentId} | {row.FullName,-20} | {row.City}, {row.Country}");
            }
        }


        public void GetAllEnrollmentsUsingSelectMany()
        {
            var report = _context.Students
                .SelectMany(
                    student => student.Enrollments,
                    (student, enrollment) => new
                    {
                        StudentName =
                            student.FirstName + " " + student.LastName,

                        enrollment.CourseId,
                        enrollment.Status
                    })
                .ToList();

            foreach (var item in report)
            {
                Console.WriteLine(
                    $"{item.StudentName} - {item.CourseId} - {item.Status}");
            }
        }


        // =========================
        // Subqueries
        // =========================

        public void GetCoursesPricedAboveAverage()
        {
            var courses = _context.Courses
                .Where(c =>
                    c.Price >
                    _context.Courses.Average(x => x.Price))
                .OrderBy(c => c.Price)
                .ToList();

            foreach (var course in courses)
            {
                Console.WriteLine(
                    $"{course.Code} - {course.Title} - {course.Price}");
            }
        }
    }
}