using BCrypt.Net;
using System.ComponentModel.DataAnnotations;

namespace SmartWater_Backend_Mvc.Models
{
    public class Login
    {
        public int Id { get; set; }
        [Required]
        [StringLength(6)]
        public string? Stuff_Id { get; set; }

        public string? Role { get; set; }

        [Required]
        public string? Password { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        public string? Fullnames { get; set; } 
    }
}
