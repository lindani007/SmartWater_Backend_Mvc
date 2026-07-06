using System.ComponentModel.DataAnnotations;

namespace SmartWater_Backend_Mvc.Models
{
    public class Employee
    {
        public string? Employer_Number { get; set; }
        [Required]
        public string? Employee_FullNames { get; set; }
        [Required]
        [Phone]
        public string? Employer_PhoneNumber { get; set; }
        [Required]
        [EmailAddress]
        public string? Employer_Email { get; set; }
        [Required]
        public string? Employee_Address { get; set; }
        [Required]
        public string? Employer_Role { get; set; }
        [Required]
        [StringLength(13)]
        public string? Employer_IdNumber { get; set; }
    }
}
