using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Smart_Water_Consumption_Service.Models
{
    public class ApplianceConsumption
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int Appliance_Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Appliance_Name { get; set; } = string.Empty;

        public double? Daily_Usage { get; set; }
        public double? Night_Usage { get; set; }

        public double? Cost { get; set; }

        public DateTime? Date { get; set; } = DateTime.UtcNow;

        [StringLength(50)]
        public string? Daily_Status { get; set; } 

        [StringLength(50)]
        public string? Night_Status { get; set; }

        public double? Leakage_Percent { get; set; }

        public string Account { get; set; } = string.Empty;
    }
}
