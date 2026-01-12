using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MaharaFinalVersion.Data;
using MaharaFinalVersion.Models;
using System.Security.Claims;
using System.Linq;

namespace MaharaFinalVersion.Controllers
{
    [Authorize]
    public class SessionsController : Controller
    {
        private readonly Mahara2DbContext _context;

        public SessionsController(Mahara2DbContext context)
        {
            _context = context;
        }

        // ------------------------------
        // Index – show sessions created by logged-in user
        // ------------------------------
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Account");

            var sessions = await _context.Sessions
                .Include(s => s.StudentSession)
                    .ThenInclude(ss => ss.Student)
                .Include(s => s.Creator)
                .Where(s => s.CreatorId == userId)
                .ToListAsync();

            return View(sessions);
        }

        // ------------------------------
        // Create Session
        // ------------------------------
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Session session)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Content("Error: User is not logged in!");
            if (!ModelState.IsValid) return View(session);

            session.CreatorId = userId;
            session.CreatedAt = DateTime.Now;

            _context.Sessions.Add(session);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // ------------------------------
        // Details
        // ------------------------------
        public async Task<IActionResult> Details(int id)
        {
            var session = await _context.Sessions
                .Include(s => s.Creator)
                .Include(s => s.StudentSession)
                    .ThenInclude(ss => ss.Student)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (session == null) return NotFound();
            return View(session);
        }

        // ------------------------------
        // Edit
        // ------------------------------
        public async Task<IActionResult> Edit(int id)
        {
            var session = await _context.Sessions.FindAsync(id);
            if (session == null) return NotFound();
            if (session.CreatorId != User.FindFirstValue(ClaimTypes.NameIdentifier)) return Forbid();
            return View(session);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Session editedSession)
        {
            if (id != editedSession.Id) return BadRequest();
            if (!ModelState.IsValid) return View(editedSession);

            var session = await _context.Sessions.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
            if (session == null) return NotFound();

            editedSession.CreatorId = session.CreatorId;
            editedSession.CreatedAt = session.CreatedAt;

            _context.Sessions.Update(editedSession);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // ------------------------------
        // Delete
        // ------------------------------
        public async Task<IActionResult> Delete(int id)
        {
            var session = await _context.Sessions.FindAsync(id);
            if (session == null) return NotFound();
            if (session.CreatorId != User.FindFirstValue(ClaimTypes.NameIdentifier)) return Forbid();

            _context.Sessions.Remove(session);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // ------------------------------
        // Test session creation
        // ------------------------------
        [HttpGet]
        public async Task<IActionResult> TestCreateSession()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var testSession = new Session
            {
                Title = "جلسة اختبار",
                Skill = "تقني",
                Description = "جلسة اختبارية للتحقق من حفظ البيانات",
                CreatorId = userId,
                CreatedAt = DateTime.Now
            };

            _context.Sessions.Add(testSession);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // ------------------------------
        // Live Session
        // ------------------------------
        public async Task<IActionResult> LiveSession(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var session = await _context.Sessions
                .Include(s => s.Creator)
                .Include(s => s.StudentSession)
                    .ThenInclude(ss => ss.Student)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (session == null) return NotFound();

            var participants = session.StudentSession.Select(ss => new Participant
            {
                Id = ss.StudentId,
                Name = ss.Student?.FullName ?? "Unknown",
                IsHost = ss.StudentId == session.CreatorId
            }).ToList();

            // If your DbContext does not have SessionInteractions, comment this block:
            var messages = await _context.SessionInteractions
    .Include(si => si.User) // ensure User is loaded
    .Where(si => si.SessionId == id)
    .Select(m => new ChatMessage
    {
        Sender = m.User != null ? m.User.FullName : "Unknown",
        Message = m.Comment,
        Time = m.CreatedAt.ToString("HH:mm")
    })
    .ToListAsync();

            var liveSession = new LiveSessionModel
            {
                Id = session.Id,
                Title = session.Title,
                Instructor = session.Creator.FullName,
                CreatorId = session.CreatorId,
                Participants = participants,
                Messages = messages,
                IsHost = userId == session.CreatorId
            };

            return View(liveSession);
        }

        // ------------------------------
        // Explore
        // ------------------------------
        [AllowAnonymous]
        public IActionResult Explore()
        {
            var sessions = _context.Sessions
                .Include(s => s.Creator)
                .Include(s => s.StudentSession)
                .Take(10)
                .ToList();

            return View(sessions);
        }

        
        
        
    }
    
}
