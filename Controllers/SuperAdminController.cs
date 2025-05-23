using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class SuperAdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SuperAdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index() => View(_context.Plans.ToList());

        [HttpGet]
        public IActionResult CreatePlan() => View();

        [HttpPost]
        public IActionResult CreatePlan(Plan plan)
        {
            if (ModelState.IsValid)
            {
                _context.Plans.Add(plan);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(plan);
        }
    }

}
