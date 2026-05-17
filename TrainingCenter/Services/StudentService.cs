using Microsoft.EntityFrameworkCore;
using TrainingCenter.Data;
using TrainingCenter.Entities;

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
                .AsNoTracking()
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
                .AsNoTracking()
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
                .AsNoTracking()
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
                .AsNoTracking()
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
                .AsNoTracking()
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
                .AsNoTracking()                
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
                .AsNoTracking()
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
                .AsNoTracking()
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


        // =========================
        // Pagination
        // =========================

        public void GetStudentsPaged(int pageNumber, int pageSize)
        {
            var pagedStudents = 
                _context.Students
                    .AsNoTracking()
                    .OrderBy(s => s.StudentId)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(s => new { s.StudentId, Name = s.FirstName + " " + s.LastName })
                    .ToList();

            if (!pagedStudents.Any())
            {
                Console.WriteLine("No students found in this page.");
                return;
            }

            foreach (var s in pagedStudents)
            {
                Console.WriteLine($"ID: {s.StudentId,-3} | Name: {s.Name}");
            }
        }


        // =========================
        // Tracked Entity Update (Change Tracker Behavior)
        // =========================

        public void UpdateTrackedStudentStatus(int studentId)
        {
            var student = _context.Students
                .FirstOrDefault(s => s.Status == "Active" && s.StudentId == studentId);

            if (student == null)
                return;

            PrintState("Before Modification", student);

            student.Status = "Suspended";

            PrintState("After Modification", student);

            int affectedRows = _context.SaveChanges();

            Console.WriteLine($"Affected Rows: {affectedRows}");

            Console.WriteLine($"Student {studentId} has been Suspended.");

        }


        // =========================
        // Tracking vs AsNoTracking (Update Behavior Comparison)
        // =========================
        public void UpdateUntrackedStudentStatus(int studentId)
        {
            var student = _context.Students
                .AsNoTracking()
                .FirstOrDefault(s => s.StudentId == studentId);

            if (student == null)
                return;

            PrintState("Loaded Entity", student);

            student.Status = "Suspended";

            PrintState("After Modification (Not Tracked)", student);

            int affectedRows = _context.SaveChanges();

            Console.WriteLine($"Affected Rows: {affectedRows}");
            Console.WriteLine("No UPDATE executed because entity is not tracked.");
        }

        private void PrintState<T>(string label, T entity)
        {
            Console.WriteLine($"{label}: {_context.Entry(entity).State}");
        }


        public class UpdateStudentStatusDto
        {
            public int StudentId { get; set; }
            public string Status { get; set; } = string.Empty;
        }



        // =========================
        // Detached Update Pattern - Load Then Update (Safe API Style)
        // =========================
        public void UpdateStudentStatusSafe(UpdateStudentStatusDto dto)
        {
            var student = _context.Students
                .FirstOrDefault(s => s.StudentId == dto.StudentId);

            if (student == null)
                return;

            student.Status = dto.Status;

            int affectedRows = _context.SaveChanges();

            Console.WriteLine($"Affected Rows: {affectedRows}");
        }



        // =========================
        // Detached Entity Update - Attach Pattern
        // =========================

        public void UpdateStudentStatusUsingAttach(UpdateStudentStatusDto dto)
        {
            var student = new Student
            {
                StudentId = dto.StudentId,
                Status = dto.Status
            };

            // Attach detached entity coming from API request
            _context.Attach(student);

            _context.Entry(student).Property(s => s.Status).IsModified = true;

            int affectedRows = _context.SaveChanges();

            Console.WriteLine($"Affected Rows: {affectedRows}");
        }
    }
}
