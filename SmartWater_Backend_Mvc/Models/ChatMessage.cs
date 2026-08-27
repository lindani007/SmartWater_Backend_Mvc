namespace SmartWater_Backend_Mvc.Models
{
    public class ChatMessage
    {
        public int ChatId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string ChatTitle { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public List<MyMessage> Message { get; set; } = new List<MyMessage>();
    }

    public class MyMessage
    {
        public bool IsUser { get; set; } = false;
        public string Message { get; set; } = string.Empty;
    }
}
