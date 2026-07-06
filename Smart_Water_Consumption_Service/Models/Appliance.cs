using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Smart_Water_Consumption_Service.Models
{
    public class Appliance
    {
        [Key]
        public int Appliance_Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Appliance_Name { get; set; } = string.Empty; 

        [StringLength(50)]
        public string? Appliance_MeterNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string Owner_AccountNumber { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        [DisplayName("Number of people using it")]
        public int NumberOfPeopleUsingIt { get; set; }
    }
}
