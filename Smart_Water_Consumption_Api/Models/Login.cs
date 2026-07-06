using System.ComponentModel.DataAnnotations;

namespace Smart_Water_Consumption_Api.Models
{
    public class Login
    {
        [Key]
        public int Id { get; set; }

        [StringLength(50)]
        public string? Account_Number { get; set; }

        [StringLength(6)]
        public string? Staff_Id { get; set; } 

        [EmailAddress]
        [StringLength(100)]
        public string? Email_Address { get; set; }

        [StringLength(12)]
        public string? Phone_Number { get; set; }

        [Required]
        [StringLength(255)] 
        public string Password { get; set; } = string.Empty;

        [Required]
        [StringLength(30)]
        public string Role { get; set; } = "citizen";

        public string? FullNames { get; set; }
    }
}
