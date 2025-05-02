using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDo.Data;
using ToDo.Models;

namespace ToDo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ToDoContext _context;
        public HomeController(ToDoContext context) => _context = context;
        public IActionResult Index(string id)
        {
            var filters = new Filters(id);
            ViewBag.Filters = filters;

            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Statuses = _context.Statuses.ToList();
            ViewBag.DueFilters = Filters.DueFilterValues;

            IQueryable<ToDoItem> query = _context.ToDoItems
                .Include(t => t.Category)
                .Include(t => t.Status);

            if (filters.HasCategory)
            {
                query = query.Where(t => t.CategoryId == filters.CategoryId);
            }
            if (filters.HasStatus)
            {
                query = query.Where(t => t.StatusId == filters.StatusId);
            }
            if (filters.HasDue)
            {
                var today = DateTime.Today;
                if (filters.IsPast)
                {
                    query = query.Where(t => t.DueDate < today);
                }
                else if (filters.IsFuture)
                {
                    query = query.Where(t => t.DueDate > today);
                }
                else if (filters.IsToday)
                {
                    query = query.Where(t => t.DueDate == today);
                }
            }
            var tasks = query.OrderBy(t => t.DueDate).ToList();
            return View(tasks);
        }

        [HttpGet]
        public IActionResult Add()
        {
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Statuses = _context.Statuses.ToList();
            var toDoItem = new ToDoItem { StatusId = "open" };

            return View(toDoItem);
        }

        [HttpPost]
        public IActionResult Add(ToDoItem toDoItem)
        {
            if (ModelState.IsValid)
            {
                _context.ToDoItems.Add(toDoItem);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Categories = _context.Categories.ToList();
                ViewBag.Statuses = _context.Statuses.ToList();
                return View(toDoItem);
            }
        }

        [HttpPost]
        public IActionResult Filter(string[] filter)
        {
            string id = string.Join("-", filter);
            return RedirectToAction("Index", new { ID = id });
        }

        [HttpPost]
        public IActionResult MarkComplete([FromRoute] string id, ToDoItem toDoItemSelected)
        {
            toDoItemSelected = _context.ToDoItems.Find(toDoItemSelected.Id)!;

            if (toDoItemSelected is not null)
            {
                toDoItemSelected.StatusId = "closed";
                _context.SaveChanges();
            }
            return RedirectToAction("Index", new { ID = id });
        }

        [HttpPost]
        public IActionResult DeleteComplete(string id)
        {
            var toDelete = _context.ToDoItems.Where(t =>  t.StatusId == "closed").ToList();

            foreach (var item in toDelete)
            {
                _context.Remove(item);
            }

            _context.SaveChanges();

            return RedirectToAction("Index", new { ID = id });
        }
    }
}