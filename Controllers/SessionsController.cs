using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MaharaFinalVersion.Data;
using MaharaFinalVersion.Models;
using System.Security.Claims;

namespace MaharaFinalVersion.Controllers
{
    [Authorize] // only logged-in users can access
    public class SessionsController : Controller
    {
        private readonly Mahara2DbContext _context;

        public SessionsController(Mahara2DbContext context)
        {
            _context = context;
        }

  
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            var sessions = await _context.Sessions
                .Include(s => s.StudentSession)
                .Include(s => s.Creator)
                .Where(s => s.CreatorId == userId)
                .ToListAsync();

            return View(sessions);
        }

   
        public IActionResult Create()
        {
            return View();
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Session session)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Content("Error: User is not logged in!");

            if (!ModelState.IsValid)
                return View(session);

            try
            {
                session.CreatorId = userId; // assign logged-in user as creator
                session.CreatedAt = DateTime.Now;

                _context.Sessions.Add(session);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index)); // redirect to جلساتي
            }
            catch (DbUpdateException dbEx)
            {
                return Content($"Database error: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
        }


        public async Task<IActionResult> Details(int id)
        {
            var session = await _context.Sessions
                .Include(s => s.Creator)
                .Include(s => s.StudentSession)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (session == null) return NotFound();

            return View(session);
        }

   
        public async Task<IActionResult> Edit(int id)
        {
            var session = await _context.Sessions.FindAsync(id);
            if (session == null) return NotFound();

            if (session.CreatorId != User.FindFirstValue(ClaimTypes.NameIdentifier))
                return Forbid();

            return View(session);
        }

     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Session editedSession)
        {
            if (id != editedSession.Id) return BadRequest();

            if (!ModelState.IsValid) return View(editedSession);

            try
            {
            
                var session = await _context.Sessions.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
                if (session == null) return NotFound();

          
                editedSession.CreatorId = session.CreatorId;
                editedSession.CreatedAt = session.CreatedAt;

                _context.Sessions.Update(editedSession);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                return Content($"Database error: {ex.InnerException?.Message ?? ex.Message}");
            }
        }


        public async Task<IActionResult> Delete(int id)
        {
            var session = await _context.Sessions.FindAsync(id);
            if (session == null) return NotFound();

            if (session.CreatorId != User.FindFirstValue(ClaimTypes.NameIdentifier))
                return Forbid();

            _context.Sessions.Remove(session);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

  
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
   public IActionResult LiveSession()
{
    var liveSession = new LiveSessionModel
    {
        Title = "جلسة تعلم C# مباشرة",
        Instructor = "رحاف",
        Participants = new List<Participant>
        {
            new Participant { Name = "رحاف", IsHost = true, IsSpeaking = true },
            new Participant { Name = "محمد", IsHost = false, IsSpeaking = false },
            new Participant { Name = "نورة", IsHost = false, IsSpeaking = true }
        },
        Messages = new List<ChatMessage>
        {
            new ChatMessage { Sender = "رحاف", Message = "مرحبا بالجميع!", Time = "10:00" },
            new ChatMessage { Sender = "محمد", Message = "مرحبا!", Time = "10:01" },
            new ChatMessage { Sender = "نورة", Message = "جاهزة للدرس", Time = "10:02" }
        }
    };

    return View(liveSession);
}

        [AllowAnonymous] 
public IActionResult Explore()
{
    var sessions = new List<Session>
    {
        new Session
        {
            Id = 1,
            Title = "شرح Git و GitHub للمبتدئين",
            Skill = "Technical",
            Description = "مقدمة عملية على Git و GitHub",
            CreatorId = "1",
            IsLive = true,
            StartTime = DateTime.Now.AddMinutes(-20),
            Duration = 60,
            StudentSession = new List<StudentSession>
            {
                new StudentSession(),
                new StudentSession(),
                new StudentSession()
            }
        },
        new Session
        {
            Id = 2,
            Title = "تصميم واجهات باستخدام Figma",
            Skill = "Design",
            Description = "تعلم أساسيات تصميم UI/UX",
            CreatorId = "2",
            IsLive = false,
            StartTime = DateTime.Now.AddHours(2),
            Duration = 90,
            StudentSession = new List<StudentSession>
            {
                new StudentSession(),
                new StudentSession()
            }
        },
        new Session
        {
            Id = 3,
            Title = "Python Data Analysis",
            Skill = "Technical",
            Description = "تحليل البيانات باستخدام Python",
            CreatorId = "3",
            IsLive = true,
            StartTime = DateTime.Now.AddMinutes(-10),
            Duration = 75,
            StudentSession = new List<StudentSession>
            {
                new StudentSession(),
                new StudentSession(),
                new StudentSession(),
                new StudentSession()
            }
        }
    };

    return View(sessions);
}


    }
}
        
        
    
