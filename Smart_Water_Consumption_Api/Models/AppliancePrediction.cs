using System.ComponentModel.DataAnnotations;

namespace Smart_Water_Consumption_Api.Models
{
    public class AppliancePrediction
    {
        [Key]
        public int Id { get; set; }
        [StringLength(100)]
        public string Appliance_Name { get; set; } = string.Empty;
        public string Owner_AccountNumber { get; set; } = string.Empty;
        public double ActualConsumptionKl { get; set; }
        public double PredictedConsumptionKl { get; set; }
        public double EstimatedCost { get; set; }
        public DateTime MonthYear { get; set; }
    }
}
