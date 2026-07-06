using System.ComponentModel.DataAnnotations;

namespace Smart_Water_Consumption_Api.Models
{
    public class RegionConsumption
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int Region_Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Region_Name { get; set; } = string.Empty;

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
