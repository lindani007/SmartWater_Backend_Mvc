using System.ComponentModel.DataAnnotations;

namespace Smart_Water_Consumption_Api.Models
{
    public class SubRegionPrediction
    {
        [Key]
        public int Id { get; set; }
        public int SubRegionId { get; set; }
        public int RegionId { get; set; }
        public string SubRegionName { get; set; } = string.Empty;
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
