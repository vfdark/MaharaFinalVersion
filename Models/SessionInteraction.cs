using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MaharaFinalVersion.Models
{
    public class SessionInteraction
    {
        [Key]
        public int Id { get; set; }

        public int SessionId { get; set; }
        public virtual Session Session { get; set; }

        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
