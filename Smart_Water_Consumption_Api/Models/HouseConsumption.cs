using System.ComponentModel.DataAnnotations;

namespace Smart_Water_Consumption_Api.Models
{
    public class HouseConsumption
    {
        [Key]
        public int Id { get; set; }
        [StringLength(50)]
        public string Household_ID { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string Owner_AccountNumber { get; set; } = string.Empty;

        public double? Water_Received { get; set; }
        public double? Total_WaterUsed { get; set; }
        public double? Leakage_Percent { get; set; }
        public double? Daily_Usage { get; set; }
        public double? Night_Usage { get; set; }
        public double? Spike_Usage { get; set; }
        public DateTime? Date { get; set; } = DateTime.UtcNow;
        public int? Sub_RegionId { get; set; }
        public double? Extepected_Amount { get; set; }
        public double? Actual_Amount { get; set; }
        public double? OutStanding_Amount { get; set; }
        public double? FreeWater { get; set; }

        [StringLength(30)]
        public string? Payment_Status { get; set; }
    }
}
