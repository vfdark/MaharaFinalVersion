namespace MaharaFinalVersion.Models;

    public class Participant
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsHost { get; set; }
        public bool IsSpeaking { get; set; }
        public User Student { get; set; } = null!;  // or = new User(); if you prefer

    }