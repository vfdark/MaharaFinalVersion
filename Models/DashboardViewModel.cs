using System;
using System.Collections.Generic;

namespace MaharaFinalVersion.Models
{
    public class DashboardViewModel
    {
        public int CompletedSessions { get; set; }
        public int LearningHours { get; set; }
        public int AchievementsCount { get; set; }
        public int ProjectsCount { get; set; }

        public List<SkillProgress> Skills { get; set; } = new List<SkillProgress>();
        public List<Session> UpcomingSessions { get; set; } = new List<Session>();
        public List<Project> Projects { get; set; } = new List<Project>();
        public List<Chat> Chats { get; set; } = new List<Chat>();
    }

    public class SkillProgress
    {
        public string Name { get; set; } = string.Empty;
        public int Percent { get; set; }
    }

    public class Project
    {
        public string Title { get; set; } = string.Empty;
        public int CompletedTasks { get; set; }
        public int TotalTasks { get; set; }
        public int Progress { get; set; }
    }

    public class Chat
    {
        public string UserName { get; set; } = string.Empty;
        public string LastMessage { get; set; } = string.Empty;
    }
}
