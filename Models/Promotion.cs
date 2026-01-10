using System;
using System.ComponentModel.DataAnnotations;

namespace MaharaFinalVersion.Models
{
    public class Promotion
    {
        [Key]
        public int Id { get; set; }

        public string StudentId { get; set; }
        public virtual ApplicationUser Student { get; set; }

        public DateTime PromotedAt { get; set; } = DateTime.Now;
    }
}