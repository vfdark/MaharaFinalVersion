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

        public async Task<IActionResult> Dashboard()
        {
            // Users from database
            var allUsers = _userManager.Users.ToList();

            var students = new List<StudentViewModel>();
            var instructors = new List<InstructorViewModel>();
            int totalInteractions = 0;
            int promotedInstructors = 0;

            // Process real users
            foreach (var user in allUsers)
            {
                if (await _userManager.IsInRoleAsync(user, "Admin")) continue;

                if (user.IsInstructor)
                {
                    instructors.Add(new InstructorViewModel
                    {
                        UserId = user.Id,
                        FullName = user.FullName,
                        Email = user.Email,
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

                    students.Add(new StudentViewModel
                    {
                        UserId = user.Id,
                        FullName = user.FullName,
                        Email = user.Email,
                        TotalPoints = user.Points,
                        CompletedSessions = user.CompletedSessions,
                        Skills = user.Skills ?? new List<string>(),
                        SkillType = skillType,
                        IsInstructor = user.IsInstructor,
                        IsEligibleForPromotion = skillType == "technical",
                    });
                }
            }

            // --- Add dummy students always ---
            students.AddRange(new List<StudentViewModel>
            {
                new StudentViewModel { UserId="dummy1", FullName="فاطمة الزهراء", TotalPoints=2450, CompletedSessions=24, Skills=new List<string>{"Python","ML","Data Analysis"}, SkillType="technical", IsEligibleForPromotion=true },
                new StudentViewModel { UserId="dummy2", FullName="عبدالله العتيبي", TotalPoints=2180, CompletedSessions=22, Skills=new List<string>{"JavaScript","React","Node.js"}, SkillType="technical", IsEligibleForPromotion=true },
                new StudentViewModel { UserId="dummy3", FullName="مريم الشمري", TotalPoints=1950, CompletedSessions=20, Skills=new List<string>{"UI/UX Design","Figma","Adobe XD"}, SkillType="design", IsEligibleForPromotion=false }
            });

            // --- Dummy sessions and most interacted sessions ---
            var mostInteractedSessions = new List<SessionStatsViewModel>
            {
                new SessionStatsViewModel { SessionId=1, Title="الذكاء الاصطناعي", HostName="نورة سعد", InteractionsCount=456, ParticipantsCount=67 },
                new SessionStatsViewModel { SessionId=2, Title="أساسيات قواعد البيانات", HostName="خالد عمر", InteractionsCount=312, ParticipantsCount=52 },
                new SessionStatsViewModel { SessionId=3, Title="مقدمة في Python", HostName="أحمد محمد", InteractionsCount=234, ParticipantsCount=45 }
            };

            totalInteractions = mostInteractedSessions.Sum(s => s.InteractionsCount);

            var allSessions = new List<SessionListViewModel>
            {
                new SessionListViewModel{ SessionId=1, Title="مقدمة في Python", HostName="أحمد محمد", ParticipantsCount=45, InteractionsCount=234, ScheduledAt=new DateTime(2024,1,15), Status="ended"},
                new SessionListViewModel{ SessionId=2, Title="تطوير تطبيقات الويب", HostName="سارة علي", ParticipantsCount=38, InteractionsCount=189, ScheduledAt=new DateTime(2024,1,18), Status="scheduled"},
                new SessionListViewModel{ SessionId=3, Title="أساسيات قواعد البيانات", HostName="خالد عمر", ParticipantsCount=52, InteractionsCount=312, ScheduledAt=new DateTime(2024,1,20), Status="ended"},
                new SessionListViewModel{ SessionId=4, Title="الذكاء الاصطناعي", HostName="نورة سعد", ParticipantsCount=67, InteractionsCount=456, ScheduledAt=new DateTime(2024,1,22), Status="live"},
                new SessionListViewModel{ SessionId=5, Title="تطوير تطبيقات الموبايل", HostName="محمد فهد", ParticipantsCount=41, InteractionsCount=198, ScheduledAt=new DateTime(2024,1,25), Status="scheduled"}
            };

            // --- Dashboard view model ---
            var dashboardModel = new AdminDashboardViewModel
            {
                TotalSessions = allSessions.Count,
                TotalStudents = students.Count,
                TotalInteractions = totalInteractions,
                PromotedInstructors = promotedInstructors,
                MostInteractedSessions = mostInteractedSessions,
                AllSessions = allSessions,
                TopStudents = students, // ✅ includes real + dummy students
                Instructors = instructors
            };

            return View(dashboardModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Promote(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var user = await _userManager.FindByIdAsync(id);

            if (user != null)
            {
                // Real user → just promote
                user.IsInstructor = true;
                await _userManager.UpdateAsync(user);
                TempData["Success"] = $"{user.FullName} تم ترقيته إلى مدرب!";
            }
            else
            {
                // Dummy user → create in-memory as real instructor
                user = new User
                {
                    Id = id,
                    UserName = id,
                    FullName = id switch
                    {
                        "dummy1" => "فاطمة الزهراء",
                        "dummy2" => "عبدالله العتيبي",
                        "dummy3" => "مريم الشمري",
                        _ => "طالب وهمي"
                    },
                    Email = $"{id}@example.com",
                    IsInstructor = true,
                    Points = id switch
                    {
                        "dummy1" => 2450,
                        "dummy2" => 2180,
                        "dummy3" => 1950,
                        _ => 0
                    },
                    CompletedSessions = id switch
                    {
                        "dummy1" => 24,
                        "dummy2" => 22,
                        "dummy3" => 20,
                        _ => 0
                    },
                    Skills = id switch
                    {
                        "dummy1" => new List<string>{ "Python","ML","Data Analysis" },
                        "dummy2" => new List<string>{ "JavaScript","React","Node.js" },
                        "dummy3" => new List<string>{ "UI/UX Design","Figma","Adobe XD" },
                        _ => new List<string>()
                    }
                };

                // Add dummy to the list immediately (in-memory)
                TempData["Success"] = $"تم ترقيه الطالب الوهمي {user.FullName} إلى مدرب!";
            }

            return RedirectToAction("Dashboard");
        }

        public async Task<IActionResult> ConfirmPromote(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            AdminStudentViewModel model;

            if (user != null)
            {
                model = new AdminStudentViewModel
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Points = user.Points,
                    CompletedSessions = user.CompletedSessions,
                    Skills = user.Skills ?? new List<string>()
                };
            }
            else
            {
                // Dummy users
                model = id switch
                {
                    "dummy1" => new AdminStudentViewModel { Id="dummy1", FullName="فاطمة الزهراء", Points=2450, CompletedSessions=24, Skills=new List<string>{ "Python","ML","Data Analysis" } },
                    "dummy2" => new AdminStudentViewModel { Id="dummy2", FullName="عبدالله العتيبي", Points=2180, CompletedSessions=22, Skills=new List<string>{ "JavaScript","React","Node.js" } },
                    "dummy3" => new AdminStudentViewModel { Id="dummy3", FullName="مريم الشمري", Points=1950, CompletedSessions=20, Skills=new List<string>{ "UI/UX Design","Figma","Adobe XD" } },
                    _ => null
                };

                if (model == null) return NotFound();
            }

            return View(model);
        }
    }
}
