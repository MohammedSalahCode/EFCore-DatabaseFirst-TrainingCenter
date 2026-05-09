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

    }
}
