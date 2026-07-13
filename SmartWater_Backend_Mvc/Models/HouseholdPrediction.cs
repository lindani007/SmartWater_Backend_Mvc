namespace SmartWater_Backend_Mvc.Models
{
    public class HouseholdPrediction
    {
        public int SubRegionId { get; set; }
        public string? SubRegionName { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public double PredictedHourConsumptionKl { get; set; }
        public double PredictedWeekConsumptionKl { get; set; }
        public double ActualMonthConsumptionKl { get; set; }
        public double PredictedMonthConsumptionKl { get; set; }
        public double EstimatedHourCost { get; set; }
        public double EstimatedWeekCost { get; set; }
        public double EstimatedMonthCost { get; set; }
    }
}
