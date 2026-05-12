using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TrainingCenter.Data;
using TrainingCenter.Services;

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
    .LogTo(Console.WriteLine, LogLevel.Information)
    .EnableSensitiveDataLogging()
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


var studentService = new StudentService(context);
var statisticsService = new StatisticsService(context);


// =========================
// Student Queries (Core LINQ)
// =========================

studentService.GetFirstActiveStudentOrDefault();
studentService.GetStudentByEmail("notfound@student.com");

studentService.GetStudentByIdUsingFind(1);
studentService.GetStudentByIdUsingFirstOrDefault(1);

studentService.GetStudentNamesOnly();
studentService.GetActiveStudentsProjectedSorted();


// =========================
// Statistics (Aggregations)
// =========================

statisticsService.CheckActiveStudentsExist();
statisticsService.CheckAllCoursesHaveValidPrice();

statisticsService.GetActiveStudentsCount();
statisticsService.GetAverageEnrollmentProgress();
statisticsService.GetTotalCoursesDuration();

statisticsService.GetMinAndMaxCoursePrices();


// =========================
// Reporting Queries (GroupBy / Distinct)
// =========================

Console.WriteLine("======== REPORTING ========");
Console.WriteLine();

statisticsService.GetDistinctStudentStatuses();
Console.WriteLine();

statisticsService.GetStudentsCountPerStatus();
Console.WriteLine();

statisticsService.GetStatusesHavingMoreThanTenStudents();

Console.WriteLine();


// =========================
// Related Data Loading
// =========================

Console.WriteLine("======== RELATED DATA LOADING ========");
Console.WriteLine();

statisticsService.DemonstrateNPlusOneProblem();

Console.WriteLine();

statisticsService.GetStudentsWithEnrollmentsUsingInclude();

Console.WriteLine();

statisticsService.GetStudentsWithEnrollmentCountUsingProjection();

Console.WriteLine();

statisticsService.GetStudentsWithEnrollmentsAndCourses();

Console.WriteLine();

statisticsService.GetStudentsSummaryReport();

Console.WriteLine();
Console.WriteLine("Execution completed successfully.");
