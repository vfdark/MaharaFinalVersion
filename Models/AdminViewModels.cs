namespace MaharaFinalVersion.Models
{
    public class AdminDashboardViewModel
    {
        public int TotalSessions { get; set; }
        public int TotalStudents { get; set; }
        public int TotalInteractions { get; set; }
        public int PromotedInstructors { get; set; }
        public List<SessionStatsViewModel> MostInteractedSessions { get; set; } = new();
        public List<SessionListViewModel> AllSessions { get; set; } = new();
        public List<StudentViewModel> TopStudents { get; set; } = new();
        public List<InstructorViewModel> Instructors { get; set; } = new();
    }

    public class SessionStatsViewModel
    {
        public int SessionId { get; set; }
        public string Title { get; set; }
        public string HostName { get; set; }
        public int InteractionsCount { get; set; }
        public int ParticipantsCount { get; set; }
    }

    public class SessionListViewModel
    {
        public int SessionId { get; set; }
        public string Title { get; set; }
        public string HostName { get; set; }
        public int ParticipantsCount { get; set; }
        public int InteractionsCount { get; set; }
        public DateTime? ScheduledAt { get; set; }
        public string Status { get; set; }

        public string StatusArabic => Status switch
        {
            "live" => "جارية",
            "scheduled" => "قادمة",
            "ended" => "مكتملة",
            "cancelled" => "ملغية",
            _ => Status
        };

        public string StatusClass => Status switch
        {
            "live" => "status-live",
            "scheduled" => "status-scheduled",
            "ended" => "status-completed",
            "cancelled" => "status-cancelled",
            _ => ""
        };
    }

    public class StudentViewModel
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public int TotalPoints { get; set; }
        public int CompletedSessions { get; set; }
        public List<string> Skills { get; set; } = new();
        public string SkillType { get; set; }
        public bool IsInstructor { get; set; }
        public bool IsEligibleForPromotion { get; set; }
        public int Rank { get; set; }

        public string SkillTypeArabic => SkillType switch
        {
            "technical" => "تقنية",
            "design" => "تصميم",
            "marketing" => "تسويق",
            _ => "أخرى"
        };

        public string SkillTypeClass => SkillType switch
        {
            "technical" => "skill-technical",
            "design" => "skill-design",
            "marketing" => "skill-marketing",
            _ => "skill-other"
        };
    }

    public class InstructorViewModel
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Specialty { get; set; }
        public int SessionsCount { get; set; }
        public decimal Rating { get; set; }
        public string Status { get; set; }
        public string ProfileImage { get; set; }
    }
}
