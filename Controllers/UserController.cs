using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Controllers
{
    [Authorize(Roles = "User")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Dashboard()
        {
            var plans = _context.Plans.ToList();
            var user = await _userManager.GetUserAsync(User);
            var subscriptions = _context.Subscriptions
                .Where(s => s.UserId == user.Id).ToList();
            ViewBag.Plans = plans;
            ViewBag.Subscriptions = subscriptions;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Subscribe(int planId)
        {
            var user = await _userManager.GetUserAsync(User);
            var sub = new Subscription
            {
                PlanId = planId,
                UserId = user.Id,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(1) // simplify logic
            };
            _context.Subscriptions.Add(sub);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }
    }

}
