using System.ComponentModel.DataAnnotations;

namespace Smart_Water_Consumption_Service.Models
{
    public class Sub_RegionConsumption
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int Sub_RegionId { get; set; }

        [Required]
        [StringLength(100)]
        public string Sub_RegionName { get; set; } = string.Empty;
        public int RegionId { get; set; }
        public double? Water_Sent { get; set; }
        public double? Water_Payed { get; set; }
        public double? OutstandingWater { get; set; }
        public double? Daily_Usage { get; set; }
        public double? Night_Usage { get; set; }
        public double? Leakage_Percent { get; set; }
        public double? Spike_Usage { get; set; }
        public double? Extepected_Amount { get; set; }
        public double? Actual_Amount { get; set; }
        public double? OutStanding_Amount { get; set; }
        public double? FreeWater { get; set; }
        public DateTime? Date { get; set; } = DateTime.UtcNow;
    }
}
