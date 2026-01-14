using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MaharaFinalVersion.Models;
using MaharaFinalVersion.Data;

namespace MaharaFinalVersion.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly Mahara2DbContext _context;

        public AdminController(UserManager<User> userManager, Mahara2DbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            // --- Users ---
            var allUsers = await _userManager.Users.ToListAsync();
            var students = new List<MaharaFinalVersion.Models.StudentViewModel>();
            var instructors = new List<InstructorViewModel>();
            int promotedInstructors = 0;

            foreach (var user in allUsers)
            {
                if (await _userManager.IsInRoleAsync(user, "Admin")) continue;

                if (user.IsInstructor)
                {
                    instructors.Add(new InstructorViewModel
                    {
                        UserId = user.Id ?? "",
                        FullName = user.FullName ?? "",
                        Email = user.Email ?? "",
                        Specialty = string.Join(", ", user.Skills ?? new List<string>()),
                        SessionsCount = user.CompletedSessions,
                        Rating = 0,
                        Status = "active",
                        ProfileImage = ""
                    });
                    promotedInstructors++;
                }
                else
                {
                    var skillType = (user.Skills != null && user.Skills.Any()) ? "technical" : "design";

                    students.Add(new MaharaFinalVersion.Models.StudentViewModel
                    {
                        UserId = user.Id ?? "",
                        FullName = user.FullName ?? "",
                        Email = user.Email ?? "",
                        TotalPoints = user.Points,
                        CompletedSessions = user.CompletedSessions,
                        Skills = user.Skills ?? new List<string>(),
                        SkillType = skillType,
                        IsInstructor = user.IsInstructor,
                        IsEligibleForPromotion = skillType == "technical",
                    });
                }
            }

            // --- Dummy students ---
            students.AddRange(new List<MaharaFinalVersion.Models.StudentViewModel>
            {
                new MaharaFinalVersion.Models.StudentViewModel
                {
                    UserId = "dummy1",
                    FullName = "فاطمة الزهراء",
                    TotalPoints = 2450,
                    CompletedSessions = 24,
                    Skills = new List<string>{"Python","ML","Data Analysis"},
                    SkillType = "technical",
                    IsEligibleForPromotion = true
                },
                new MaharaFinalVersion.Models.StudentViewModel
                {
                    UserId = "dummy2",
                    FullName = "عبدالله العتيبي",
                    TotalPoints = 2180,
                    CompletedSessions = 22,
                    Skills = new List<string>{"JavaScript","React","Node.js"},
                    SkillType = "technical",
                    IsEligibleForPromotion = true
                },
                new MaharaFinalVersion.Models.StudentViewModel
                {
                    UserId = "dummy3",
                    FullName = "مريم الشمري",
                    TotalPoints = 1950,
                    CompletedSessions = 20,
                    Skills = new List<string>{"UI/UX Design","Figma","Adobe XD"},
                    SkillType = "design",
                    IsEligibleForPromotion = false
                }
            });

            // --- Fetch sessions from DB ---
            var allSessions = await _context.Sessions
                .Include(s => s.Creator)
                .Include(s => s.StudentSession) // <-- match your EF property
                .Include(s => s.Interactions)    // <-- match your EF property
                .ToListAsync();

            // Map sessions to view model
            var sessionList = allSessions.Select(s => new SessionListViewModel
            {
                SessionId = s.Id,
                Title = s.Title ?? "",
                HostName = s.Creator?.FullName ?? "Unknown",
                ParticipantsCount = s.StudentSession?.Count ?? 0,
                InteractionsCount = s.Interactions?.Count ?? 0,
                ScheduledAt = s.StartTime ?? s.CreatedAt,
                Status = s.DeletedAt != null ? "deleted" : s.IsLive ? "live" : "ended"
            }).ToList();

            int totalInteractions = sessionList.Sum(s => s.InteractionsCount);

            // --- Most interacted sessions ---
            var mostInteractedSessions = sessionList
                .OrderByDescending(s => s.InteractionsCount)
                .Take(3)
                .Select(s => new SessionStatsViewModel
                {
                    SessionId = s.SessionId,
                    Title = s.Title,
                    HostName = s.HostName,
                    ParticipantsCount = s.ParticipantsCount,
                    InteractionsCount = s.InteractionsCount
                }).ToList();

            // --- Dashboard view model ---
            var dashboardModel = new AdminDashboardViewModel
            {
                TotalSessions = sessionList.Count,
                TotalStudents = students.Count,
                TotalInteractions = totalInteractions,
                PromotedInstructors = promotedInstructors,
                MostInteractedSessions = mostInteractedSessions,
                AllSessions = sessionList,
                TopStudents = students,
                Instructors = instructors
            };

            return View(dashboardModel);
        }

        // --- Promote Student to Instructor ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Promote(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                user.IsInstructor = true;
                await _userManager.UpdateAsync(user);
                TempData["Success"] = $"{user.FullName} تم ترقيته إلى مدرب!";
            }
            else
            {
                TempData["Success"] = "تم ترقية الطالب الوهمي إلى مدرب!";
            }

            return RedirectToAction("Dashboard");
        }

        // --- Edit Session ---
      
       [HttpGet]
public async Task<IActionResult> EditSession(int sessionId)
{
    var session = await _context.Sessions
        .Include(s => s.Creator)
        .FirstOrDefaultAsync(s => s.Id == sessionId);

    if (session == null) return NotFound();

    // Map to SessionListViewModel
    var model = new SessionListViewModel
    {
        SessionId = session.Id,
        Title = session.Title,
        HostName = session.Creator?.FullName ?? "",
        Status = session.IsLive ? "live" : "ended"
        // Add more properties if needed
    };

    return View(model);
}





        // --- Edit Session POST ---
        [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> EditSession(SessionListViewModel model)
{
    if (!ModelState.IsValid) return View(model);

    var session = await _context.Sessions.FindAsync(model.SessionId);
    if (session == null) return NotFound();

    session.Title = model.Title ?? "";
    session.IsLive = model.Status == "live";

    await _context.SaveChangesAsync();
    TempData["Success"] = "تم تعديل الجلسة بنجاح!";

    return RedirectToAction("Dashboard");
}


        // --- Delete Session GET ---
        [HttpGet]
public async Task<IActionResult> DeleteSession(int id)
{
    var session = await _context.Sessions.FindAsync(id);
    if (session == null) return NotFound();

    var model = new SessionListViewModel
    {
        SessionId = session.Id,
        Title = session.Title ?? "",
        Description = session.Description ?? "",
        HostName = session.Creator?.FullName ?? "Unknown",
        ParticipantsCount = session.StudentSession?.Count ?? 0,
        InteractionsCount = session.Interactions?.Count ?? 0,
        ScheduledAt = session.StartTime ?? session.CreatedAt,
        Status = session.IsLive ? "live" : "ended"
    };

    return View(model);
}


        // --- Delete Session POST (hard delete) ---
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSession(int id, string deleteReason)
        {
            var session = await _context.Sessions.FindAsync(id);
            if (session == null) return NotFound();

            _context.Sessions.Remove(session);
            await _context.SaveChangesAsync();

            // Redirect to confirmation page
            return RedirectToAction("DeleteSessionConfirmation", new { title = session.Title, reason = deleteReason });
        }

        // --- Delete Session Confirmation ---
        [HttpGet]
        public IActionResult DeleteSessionConfirmation(string title, string reason)
        {
            ViewData["DeletedSessionTitle"] = title;
            ViewData["DeleteReason"] = reason;
            return View(); // DeleteSessionConfirmation.cshtml
        }
        // GET: Admin/ConfirmPromote/{id}
[HttpGet]
public async Task<IActionResult> ConfirmPromote(string id)
{
    if (string.IsNullOrEmpty(id))
        return NotFound();

    var user = await _userManager.FindByIdAsync(id);
    if (user == null)
        return NotFound();

    var model = new AdminStudentViewModel
    {
        Id = user.Id,
        FullName = user.FullName ?? "",
        Points = user.Points,
        CompletedSessions = user.CompletedSessions,
        Skills = user.Skills ?? new List<string>()
    };

    return View(model); // ✅ matches your view
}

    }
}