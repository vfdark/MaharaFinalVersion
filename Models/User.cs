using Microsoft.AspNetCore.Identity;

namespace MaharaFinalVersion.Models
{
    public class User : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public bool IsInstructor { get; set; } = false;
         public int Points { get; set; } = 0; 
           public bool IsTechnical { get; set; } = false;
               public List<string> Skills { get; set; } = new List<string>();
                   public int CompletedSessions { get; set; } = 0;

        // Navigation properties
    // Add this to fix CS1061

    public virtual ICollection<Session>? Sessions { get; set; }
    public virtual ICollection<StudentSession>? StudentSessions { get; set; }
    public virtual ICollection<InstructorPromotion>? InstructorPromotions { get; set; }

        public virtual ICollection<SessionInteraction> SessionInteractions { get; set; } = new List<SessionInteraction>();


        
    }
}
