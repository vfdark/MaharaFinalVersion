using Microsoft.AspNetCore.Identity;

namespace MaharaFinalVersion.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public bool IsInstructor { get; set; } = false;
    }
}
