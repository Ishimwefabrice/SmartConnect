using Microsoft.AspNetCore.Mvc;
using smartconnect.Data;

namespace smartconnect.Controllers
{
    public class DatabaseController : Controller
    {
        private readonly DbConnectionFactory _connectionFactory;

        public DatabaseController(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IActionResult> TestConnection()
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();
            return Content("Successfully connected to the auca1 database!");
        }
    }
}
