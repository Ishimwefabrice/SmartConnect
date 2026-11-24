using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using smartconnect.Data;
using smartconnect.Models;

namespace smartconnect.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        private async Task<List<Student>> GetStudentsAsync(DbConnectionFactory connectionFactory)
        {
            var students = new List<Student>();
            using var connection = connectionFactory.CreateConnection();
            await connection.OpenAsync();
            string query = @"SELECT studentID, Name, Age, Course
                             FROM student
                             ORDER BY studentID";
            using var command = new MySqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                students.Add(new Student
                {
                    StudentID = reader.GetString("studentID"),
                    Name = reader.GetString("Name"),
                    Age = reader.GetInt32("Age"),
                    Course = reader.GetString("Course")
                });
            }

            return students;
        }

        [HttpGet]
        public async Task<IActionResult> Registration([FromServices] DbConnectionFactory connectionFactory)
        {
            var viewModel = new StudentViewModel();
            viewModel.Students = await GetStudentsAsync(connectionFactory);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registration(Student student, [FromServices] DbConnectionFactory connectionFactory)
        {
            if (!ModelState.IsValid)
            {
                var invalidModel = new StudentViewModel
                {
                    Student = student,
                    Students = await GetStudentsAsync(connectionFactory)
                };
                return View(invalidModel);
            }

            try
            {
                using var connection = connectionFactory.CreateConnection();
                await connection.OpenAsync();
                string query = @"INSERT INTO student
                                 (studentID, Name, Age, Course)
                                 VALUES
                                 (@studentID, @Name, @Age, @Course)";
                using var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@studentID", student.StudentID);
                command.Parameters.AddWithValue("@Name", student.Name);
                command.Parameters.AddWithValue("@Age", student.Age);
                command.Parameters.AddWithValue("@Course", student.Course);
                await command.ExecuteNonQueryAsync();
                TempData["SuccessMessage"] = "Student registered successfully!";
                return RedirectToAction("Registration");
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1062)
                {
                    ModelState.AddModelError("Student.StudentID", "This registration number already exists.");
                }
                else
                {
                    ModelState.AddModelError("", "Database error: " + ex.Message);
                }

                var errorModel = new StudentViewModel
                {
                    Student = student,
                    Students = await GetStudentsAsync(connectionFactory)
                };
                return View(errorModel);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
