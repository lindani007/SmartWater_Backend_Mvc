using System.ComponentModel.DataAnnotations;

namespace Smart_Water_Consumption_Api.Models
{
    public class ChatMessage
    {
        [Key]
        public int ChatId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string ChatTitle { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}
