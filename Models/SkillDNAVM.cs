namespace MaharaFinalVersion.Models
{
    public class SkillDNAVM
    {
        public string FullName { get; set; }
        public int Points { get; set; }
        public int Rank { get; set; }
        public int MaxPoints { get; set; } = 3000;

        public List<SkillItemVM> Skills { get; set; } = new();
        public List<string> Achievements { get; set; } = new();
    }

    public class SkillItemVM
    {
        public string Name { get; set; }
        public int Level { get; set; } // 1–5
    }
}
