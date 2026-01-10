namespace MaharaFinalVersion.Models
{
    public class StudentSession
    {
        public int Id { get; set; }
        public string StudentId { get; set; } = string.Empty;
        public int SessionId { get; set; }
        public User Student { get; set; }
public Session Session { get; set; }
    }
}
