using System.ComponentModel.DataAnnotations;

namespace MaharaFinalVersion.Models
{
    public class UserSkill
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }
       public User User { get; set; } = null!;  // same as above


        public string SkillName { get; set; }
        public string Level { get; set; }
    }
}
