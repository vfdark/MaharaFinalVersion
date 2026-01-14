using System.Collections.Generic;

namespace MaharaFinalVersion.Models
{
    public class AdminStudentViewModel
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public int Points { get; set; }
        public bool IsTechnical { get; set; }
        public List<string> Skills { get; set; } = new List<string>();
        public int CompletedSessions { get; set; }
        public bool IsInstructor { get; set; } = false;
         public string Email { get; set; } = ""; 
    }
}
