using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MaharaFinalVersion.Models;

namespace MaharaFinalVersion.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly UserManager<User> _userManager;

        public DashboardController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var model = new DashboardViewModel
            {
                CompletedSessions = 0,
                LearningHours = 0,
                AchievementsCount = 0,
                ProjectsCount = 0,
                Skills = new List<SkillProgress>(),
                UpcomingSessions = new List<Session>(),
                Projects = new List<Project>(),
                Chats = new List<Chat>()
            };

            return View(model);
        }

 
        public async Task<IActionResult> Skills()
        {
            var user = await _userManager.GetUserAsync(User);
            return View(user);
        }

      
        [HttpPost]
        public async Task<IActionResult> AddSkill(string skill)
        {
            if (string.IsNullOrWhiteSpace(skill))
                return RedirectToAction("Skills");

            var user = await _userManager.GetUserAsync(User);

            user.Skills ??= new List<string>();

            if (!user.Skills.Contains(skill))
                user.Skills.Add(skill);

            await _userManager.UpdateAsync(user);

            return RedirectToAction("Skills");
        }

    
        [HttpPost]
        public async Task<IActionResult> RemoveSkill(string skill)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user.Skills != null && user.Skills.Contains(skill))
            {
                user.Skills.Remove(skill);
                await _userManager.UpdateAsync(user);
            }

            return RedirectToAction("Skills");
        }
        public IActionResult SkillDNA()
{
    var model = new SkillDNAVM
    {
        FullName = "أحمد محمد",
        Points = 2450,
        Rank = 3,
        Skills = new()
        {
            new SkillItemVM { Name = "React", Level = 5 },
            new SkillItemVM { Name = "Python", Level = 4 },
            new SkillItemVM { Name = "UI/UX", Level = 4 },
            new SkillItemVM { Name = "Leadership", Level = 3 }
        },
        Achievements = new()
        {
            "أفضل مستضيف",
            "10 جلسات",
            "متعلم سريع"
        }
    };

    return View(model);
}

    }
}
