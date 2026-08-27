using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Smart_Water_Consumption_Service.Models
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
