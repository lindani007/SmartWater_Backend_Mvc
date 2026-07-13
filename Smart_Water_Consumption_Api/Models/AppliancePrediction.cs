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
        public double PredictedHourConsumptionKl { get; set; }
        public double PredictedWeekConsumptionKl { get; set; }
        public double ActualMonthConsumptionKl { get; set; }
        public double PredictedMonthConsumptionKl { get; set; }
        public double EstimatedHourCost { get; set; }
        public double EstimatedWeekCost { get; set; }
        public double EstimatedMonthCost { get; set; }
        public DateTime MonthYear { get; set; }
    }
}
