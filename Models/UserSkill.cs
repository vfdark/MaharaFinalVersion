using System.ComponentModel.DataAnnotations;

namespace MaharaFinalVersion.Models
{
    public class UserSkill
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public string SkillName { get; set; }
        public string Level { get; set; }
    }
}
