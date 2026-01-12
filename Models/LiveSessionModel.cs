using System.Collections.Generic;

namespace MaharaFinalVersion.Models
{
    

    public class LiveSessionModel
    {
         public int Id { get; set; } 
        public string Title { get; set; } = string.Empty;
        public string Instructor { get; set; } = string.Empty;
         public string CreatorId { get; set; } = string.Empty; // Needed in controller
        public bool IsHost { get; set; } // Needed in controller
        public List<Participant> Participants { get; set; } = new List<Participant>();
        public List<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
        // THIS FIXES your error

    }

    
}
        
        
