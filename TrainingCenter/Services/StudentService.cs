using Microsoft.EntityFrameworkCore;
using TrainingCenter.Data;

namespace TrainingCenter.Services
{
    public class StudentService
    {
        private readonly AppDbContext _context;

        public StudentService(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        //  Retrieve All Students
        // =========================
        public void PrintAllStudents()
        {
            var query = _context.Students
                .OrderBy(s => s.StudentId);

            // Execute query
            var students = query.ToList();

            if (students.Count == 0)
            {
                Console.WriteLine("No students found.");
                return;
            }

            Console.WriteLine("Students List:");
            Console.WriteLine("--------------");

            foreach (var student in students)
            {
                Console.WriteLine(
                    $"Id: {student.StudentId}, " +
                    $"Name: {student.FirstName} {student.LastName}, " +
                    $"Email: {student.Email}, " +
                    $"Status: {student.Status}, " +
                    $"Phone: {student.PhoneNumber ?? "N/A"}");
            }

            Console.WriteLine($"Total Students: {students.Count}");
            Console.WriteLine();
        }


        // =========================
        // Retrieve Active Students With Query Inspection
        // =========================
        public void PrintActiveStudentsWithTracing()
        {
            var query = _context.Students
                .Where(s => s.Status == "Active")
                .OrderBy(s => s.StudentId);

            // Preview SQL (design-time)
            Console.WriteLine("Generated SQL:");
            Console.WriteLine("----------------");
            Console.WriteLine(query.ToQueryString());
            Console.WriteLine();

            var students = query.ToList();

            Console.WriteLine($"Active Students: {students.Count}");
            Console.WriteLine();
        }



        // =========================
        // Count Active Students (Shows runtime SQL difference)
        // =========================
        public void GetActiveStudentsCount()
        {
            var query = _context.Students
                .Where(s => s.Status == "Active");

            Console.WriteLine("Preview SQL:");
            Console.WriteLine("----------------");
            Console.WriteLine(query.ToQueryString());
            Console.WriteLine();

            int count = query.Count();

            Console.WriteLine($"Active Students Count: {count}");
            Console.WriteLine();
        }



        // =========================
        // Single Retrieval
        // =========================

        public void GetFirstActiveStudentOrDefault()
        {
            var query = _context.Students
                .Where(s => s.Status == "Active")
                .OrderBy(s => s.StudentId);

            Console.WriteLine("Preview SQL:");
            Console.WriteLine("----------------");
            Console.WriteLine(query.ToQueryString());
            Console.WriteLine();

            var student = query.FirstOrDefault();

            if (student == null)
            {
                Console.WriteLine("No active student found");
                return;
            }

            Console.WriteLine($"{student.StudentId} - {student.FirstName} {student.LastName}");
        }


        public void GetStudentByEmail(string email)
        {
            var query = _context.Students
                .Where(s => s.Email == email);

            Console.WriteLine("Preview SQL:");
            Console.WriteLine("----------------");
            Console.WriteLine(query.ToQueryString());
            Console.WriteLine();

            var student = query.SingleOrDefault();

            if (student == null)
            {
                Console.WriteLine("Student not found");
                return;
            }

            Console.WriteLine($"{student.StudentId} - {student.Email}");
        }



        // =========================
        // Primary Key Lookup
        // =========================

        public void GetStudentByIdUsingFind(int id)
        {
            var student = _context.Students.Find(id);

            if (student == null)
            {
                Console.WriteLine("Student not found.");
                return;
            }

            Console.WriteLine($"{student.StudentId} - {student.FirstName}");
        }


        public void GetStudentByIdUsingFirstOrDefault(int id)
        {
            var query = _context.Students
                .Where(s => s.StudentId == id);

            Console.WriteLine("Preview SQL:");
            Console.WriteLine("----------------");
            Console.WriteLine(query.ToQueryString());
            Console.WriteLine();

            var student = query.FirstOrDefault();

            if (student == null)
            {
                Console.WriteLine("Student not found.");
                return;
            }

            Console.WriteLine($"{student.StudentId} - {student.FirstName}");
        }


        // =========================
        // Projection
        // =========================

        public void GetStudentNamesOnly()
        {
            var students = _context.Students
                .Select(s => new
                {
                    s.FirstName,
                    s.LastName
                })
                .ToList();

            foreach (var s in students)
            {
                Console.WriteLine($"{s.FirstName} {s.LastName}");
            }
        }


        public void GetActiveStudentsProjectedSorted()
        {
            var students = _context.Students
                .Where(s => s.Status == "Active")
                .Select(s => new
                {
                    s.StudentId,
                    FullName = s.FirstName + " " + s.LastName
                })
                .OrderByDescending(s => s.StudentId)
                .ToList();

            foreach (var s in students)
            {
                Console.WriteLine($"{s.StudentId} - {s.FullName}");
            }
        }
    }
}
