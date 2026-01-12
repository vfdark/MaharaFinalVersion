using System;

namespace MaharaFinalVersion.Models
{
    public class Session
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Skill { get; set; } = string.Empty; // Technical or Non-Technical
        public string Description { get; set; } = string.Empty;
        public string CreatorId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public User? Creator { get; set; }  
        public bool IsLive { get; set; }
         public DateTime? StartTime { get; set; }
           public int? Duration { get; set; } // minutes
           

        public ICollection<StudentSession> StudentSession { get; set; } = new List<StudentSession>();
        public ICollection<SessionInteraction> Interactions { get; set; } = new List<SessionInteraction>();



    }
}
