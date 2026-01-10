using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MaharaFinalVersion.Models;

namespace MaharaFinalVersion.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<User> _userManager;

        public AdminController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        // Dashboard showing students
        public async Task<IActionResult> Dashboard()
        {
            var students = new List<AdminStudentViewModel>();

            //Get real users from the database
            var allUsers = _userManager.Users.ToList();
            foreach (var user in allUsers)
            {
                if (!await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    students.Add(new AdminStudentViewModel
                    {
                        Id = user.Id,
                        FullName = user.FullName,
                        Points = user.Points,
                        IsTechnical = user.Skills != null && user.Skills.Any(),
                        Skills = user.Skills ?? new List<string>(),
                        CompletedSessions = user.CompletedSessions
                    });
                }
            }

            // 2️⃣ Add dummy students for testing / if DB is empty
            var dummyStudents = new List<AdminStudentViewModel>
            {
                new AdminStudentViewModel
                {
                    Id = "dummy1",
                    FullName = "فاطمة الزهراء",
                    Points = 2450,
                    IsTechnical = true,
                    Skills = new List<string> { "Python", "Machine Learning", "Data Analysis" },
                    CompletedSessions = 24
                },
                new AdminStudentViewModel
                {
                    Id = "dummy2",
                    FullName = "عبدالله العتيبي",
                    Points = 2180,
                    IsTechnical = true,
                    Skills = new List<string> { "JavaScript", "React", "Node.js" },
                    CompletedSessions = 22
                },
                new AdminStudentViewModel
                {
                    Id = "dummy3",
                    FullName = "مريم الشمري",
                    Points = 1950,
                    IsTechnical = false,
                    Skills = new List<string> { "UI/UX Design", "Figma", "Adobe XD" },
                    CompletedSessions = 20
                }
            };

            // Merge real users and dummy students
            students.AddRange(dummyStudents);

            return View(students); // تمرير الـ ViewModel للـ View
        }

        // POST: Promote student to instructor
        // POST: Admin/Promote
[HttpPost("Admin/Promote")]
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
        // For dummy users
        TempData["Success"] = $"تم ترقيه الطالب الوهمي {id} إلى مدرب!";
    }

    return RedirectToAction("Dashboard");
}

    
        // GET: Confirm promotion page
        // GET: Confirm promotion page
public async Task<IActionResult> ConfirmPromote(string id)
{
    if (id == null) return NotFound();

    AdminStudentViewModel model = null;

    // Check if user exists in database
    var user = await _userManager.FindByIdAsync(id);
    if (user != null)
    {
        model = new AdminStudentViewModel
        {
            Id = user.Id,
            FullName = user.FullName,
            Points = user.Points,
            IsTechnical = user.Skills != null && user.Skills.Any(),
            Skills = user.Skills ?? new List<string>(),
            CompletedSessions = user.CompletedSessions
        };
    }
    else
    {
        //  If not, check dummy students
        var dummyStudents = new List<AdminStudentViewModel>
        {
            new AdminStudentViewModel
            {
                Id = "dummy1",
                FullName = "فاطمة الزهراء",
                Points = 2450,
                IsTechnical = true,
                Skills = new List<string> { "Python", "Machine Learning", "Data Analysis" },
                CompletedSessions = 24
            },
            new AdminStudentViewModel
            {
                Id = "dummy2",
                FullName = "عبدالله العتيبي",
                Points = 2180,
                IsTechnical = true,
                Skills = new List<string> { "JavaScript", "React", "Node.js" },
                CompletedSessions = 22
            },
            new AdminStudentViewModel
            {
                Id = "dummy3",
                FullName = "مريم الشمري",
                Points = 1950,
                IsTechnical = false,
                Skills = new List<string> { "UI/UX Design", "Figma", "Adobe XD" },
                CompletedSessions = 20
            }
        };

        model = dummyStudents.FirstOrDefault(s => s.Id == id);
    }

    if (model == null) return NotFound();

    return View(model);
}

    }

    // ViewModel 
    public class AdminStudentViewModel
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public int Points { get; set; }
        public bool IsTechnical { get; set; }
        public List<string> Skills { get; set; } = new List<string>();
        public int CompletedSessions { get; set; }
    }
    
}
