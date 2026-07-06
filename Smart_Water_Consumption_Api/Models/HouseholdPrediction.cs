using System.ComponentModel.DataAnnotations;

namespace Smart_Water_Consumption_Api.Models
{
    public class HouseholdPrediction
    {
        [Key]
        public int Id { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public double ActualConsumptionKl { get; set; }
        public double PredictedConsumptionKl { get; set; }
        public double EstimatedCost { get; set; }
        public DateTime MonthYear { get; set; }
    }
}
