using Microsoft.AspNetCore.Mvc;
using MaharaFinalVersion.Data;
using MaharaFinalVersion.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace MaharaFinalVersion.Controllers
{
    public class AchievementsController : Controller
    {
        private readonly Mahara2DbContext _context;
        private readonly UserManager<User> _userManager;

        public AchievementsController(Mahara2DbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Achievements
        public async Task<IActionResult> Index()
        {
            // Get current logged-in user
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return RedirectToAction("Login", "Account");

            // Get only this user's achievements
            var achievements = await _context.InstructorPromotions
                .Where(a => a.StudentId == currentUser.Id)
                .ToListAsync();

            return View(achievements);
        }
    }
}
