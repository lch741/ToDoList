using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using ToDoList.Models;

namespace ToDoList.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public void Insert(ToDoModel toDo)
        {
            using (SqliteConnection con = new SqliteConnection("Data Source=db.db"))
            {
                using (var tableCmd = con.CreateCommand())
                {
                    con.Open();
                    tableCmd.CommandText = "INSERT INTO ToDos (name) VALUES (@name)";
                    tableCmd.Parameters.AddWithValue("@name", toDo.Name);
                    try { 
                        tableCmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error inserting ToDo item");
                    }
                }
            }
        }
    }
}
