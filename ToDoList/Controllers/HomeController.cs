using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using ToDoList.Models;
using ToDoList.Models.View;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ToDoList.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SqliteConnection _connection;

        public HomeController(ILogger<HomeController> logger, SqliteConnection connection)
        {
            _logger = logger;
            _connection = connection;
        }

        public IActionResult Index()
        {
            var toDoViewModel = GetAllTodos();
            return View(toDoViewModel);
        }

        public RedirectResult Insert(ToDoModel toDo)
        {
            using (var tableCmd = _connection.CreateCommand())
            {
                _connection.Open();
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
            return Redirect("/");

        }

        internal ToDoViewModel GetAllTodos()
        {
            var toDoViewModel = new ToDoViewModel();
            toDoViewModel.TodoList = new List<ToDoModel>();
            using (var tableCmd = _connection.CreateCommand())
            {
                _connection.Open();
                tableCmd.CommandText = "SELECT * FROM ToDos";
                try
                {
                    using (var reader = tableCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var toDoItem = new ToDoModel
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1)
                            };
                            toDoViewModel.TodoList.Add(toDoItem);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error retrieving ToDo items");
                }
            }
            return toDoViewModel;
        }

        [HttpPost]
        public JsonResult Delete(int id) {
            using(var tableCmd = _connection.CreateCommand())
            {
                _connection.Open();
                tableCmd.CommandText = "DELETE FROM ToDos WHERE id = @id";
                tableCmd.Parameters.AddWithValue("@id", id);
                try
                {
                    tableCmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error deleting ToDo item");
                    return Json(new { success = false, message = "Error deleting ToDo item." });
                }
                return Json(new { success = true });
            }
        }

        internal ToDoModel GetById(int id)
        {
            ToDoModel toDoItem = null;
            using (var tableCmd = _connection.CreateCommand())
            {
                _connection.Open();
                tableCmd.CommandText = "SELECT * FROM ToDos WHERE id = @id";
                tableCmd.Parameters.AddWithValue("@id", id);
                try
                {
                    using (var reader = tableCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            toDoItem = new ToDoModel
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1)
                            };
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error retrieving ToDo item by ID");
                }
            }
            return toDoItem;
        }

        public RedirectResult Update(ToDoModel todo)
        {
            using (var tableCmd = _connection.CreateCommand())
            {
                _connection.Open();
                tableCmd.CommandText = "UPDATE ToDos SET name = @name WHERE id = @id";
                tableCmd.Parameters.AddWithValue("@name", todo.Name);
                tableCmd.Parameters.AddWithValue("@id", todo.Id);
                try
                {
                    tableCmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating ToDo item");
                }
            }
            return Redirect("/");
        }

        [HttpGet]
        public JsonResult PopulateForm(int id)
        {
            var todo = GetById(id);
            return Json(todo);
        }
    }
}
