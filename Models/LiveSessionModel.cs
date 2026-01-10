using System.Collections.Generic;

namespace MaharaFinalVersion.Models
{
    public class Participant
    {
        public string Name { get; set; } = string.Empty;
        public bool IsHost { get; set; }
        public bool IsSpeaking { get; set; }
    }

    public class ChatMessage
    {
        public string Sender { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
    }

    public class LiveSessionModel
    {
        public string Title { get; set; } = string.Empty;
        public string Instructor { get; set; } = string.Empty;
        public List<Participant> Participants { get; set; } = new List<Participant>();
        public List<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
    }
}
