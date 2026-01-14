namespace MaharaFinalVersion.Models
{}
public class StudentViewModel
{
    public string UserId { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public int TotalPoints { get; set; }
    public int CompletedSessions { get; set; }
    public List<string> Skills { get; set; } = new List<string>(); // <-- add this
    public string SkillType { get; set; }
    public bool IsInstructor { get; set; }
    public bool IsEligibleForPromotion { get; set; }
        public int Rank { get; set; } = 0;  // <-- Add this
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

