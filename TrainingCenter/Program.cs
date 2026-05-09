using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TrainingCenter.Data;

IConfiguration configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

string? connectionString = configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    Console.WriteLine("Connection string not found.");
    return;
}


var options = new DbContextOptionsBuilder<AppDbContext>()
    .UseSqlServer(connectionString)
    .Options;

using var context = new AppDbContext(options);


bool canConnect = context.Database.CanConnect();

if (!canConnect)
{
    Console.WriteLine("Could not connect to TrainingCenterDB.");
    return;
}

Console.WriteLine("Connected successfully to TrainingCenterDB.");
Console.WriteLine();


RetrieveAndPrintStudents(context);


/// <summary>
/// Retrieves all students from the database, prints the generated SQL,
/// then prints the returned student data.
/// </summary>
/// <param name="context">The EF Core database context.</param>
static void RetrieveAndPrintStudents(AppDbContext context)
{
    var query = context.Students
        .OrderBy(s => s.StudentId);

    PrintGeneratedSql("Students", query.ToQueryString());

    // Execute query
    var students = query.ToList();

    if (students.Count == 0)
    {
        Console.WriteLine("No students found in the database.");
        Console.WriteLine();
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

    Console.WriteLine();
    Console.WriteLine($"Total Students: {students.Count}");
    Console.WriteLine(new string('=', 70));
    Console.WriteLine();
}

static void PrintGeneratedSql(string tableName, string sqlQuery)
{
    Console.WriteLine($"Generated SQL Query for {tableName}:");
    Console.WriteLine(new string('-', 40));
    Console.WriteLine(sqlQuery);
    Console.WriteLine();
}