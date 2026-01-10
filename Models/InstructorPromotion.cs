using System;

namespace MaharaFinalVersion.Models
{
    public class InstructorPromotion
    {
        public int Id { get; set; }
        public string StudentId { get; set; } = string.Empty;
        public DateTime PromotedAt { get; set; } = DateTime.Now;
        public User Student { get; set; }
    }
}
