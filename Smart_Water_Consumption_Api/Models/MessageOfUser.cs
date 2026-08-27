using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Smart_Water_Consumption_Api.Models
{
    public class MessageOfUser
    {
        [Key]
        public int Id { get; set; }
        public int ChatId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public bool IsUser { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
