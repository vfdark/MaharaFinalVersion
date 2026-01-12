using System;
using System.ComponentModel.DataAnnotations;

namespace MaharaFinalVersion.Models
{
    public class SessionParticipant
    {
        [Key]
        public int Id { get; set; }

        public string StudentId { get; set; }
        public virtual User Student { get; set; }

        public int SessionId { get; set; }
        public virtual Session Session { get; set; }

        
    }
}