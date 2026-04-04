using Agreely.Data;
using Microsoft.AspNetCore.Mvc;

namespace Agreely.Controllers
{
    public class HomeController : Controller
    {
        private readonly DatabaseHelper _db;

        public HomeController(DatabaseHelper db)
        {
            _db = db;
        }

        public IActionResult TestConnection()
        {
            try
            {
                using var connection = _db.GetConnection();
                connection.Open();

                return Content("Connection successful!");
            }
            catch (Exception ex)
            {
                return Content("Error: " + ex.Message);
            }
        }
    }
}