using System.ComponentModel.DataAnnotations;

namespace Smart_Water_Consumption_Service.Models
{
    public class Employee
    {
        [Key]
        [StringLength(50)]
        public string Employee_Number { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string EmployeeFullNames { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string EmployeeEmail { get; set; } = string.Empty;

        [Required]
        [StringLength(15)]
        public string EmployeePhone { get; set; } = string.Empty;

        [StringLength(250)]
        public string? EmployeeAddress { get; set; }

        [Required]
        [StringLength(30)]
        public string EmployeeRole { get; set; } = string.Empty; 

        [Required]
        [StringLength(20)]
        public string EmployeeIdNumber { get; set; } = string.Empty;
    }
}
