using System.ComponentModel.DataAnnotations;

namespace Smart_Water_Consumption_Service.Models
{
    public class Owner
    {
        [Key]
        [StringLength(13)] 
        public string Owner_IdNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string Owner_Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Owner_AccountNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Owner_MeterNumber { get; set; } = string.Empty; 

        [StringLength(15)]
        public string? Owner_PhoneNumber { get; set; } 

        [EmailAddress]
        [StringLength(100)]
        public string? Owner_Email { get; set; }

        [StringLength(50)]
        public string? Household_ID { get; set; }

        public int? Sub_RegionId { get; set; }
        public string Sub_RegionName { get; set;} = string.Empty;
        public string PropertyValue { get; set; } = string.Empty;
    }
}
