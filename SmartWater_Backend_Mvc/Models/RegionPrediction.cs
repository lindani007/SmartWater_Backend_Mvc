namespace SmartWater_Backend_Mvc.Models
{
    public class RegionPrediction
    {
        public int RegionId { get; set; }
        public string RegionName { get; set; } = string.Empty;
        public double PredictedHourConsumptionKl { get; set; }
        public double PredictedWeekConsumptionKl { get; set; }
        public double ActualMonthConsumptionKl { get; set; }
        public double PredictedMonthConsumptionKl { get; set; }
        public double EstimatedHourCost { get; set; }
        public double EstimatedWeekCost { get; set; }
        public double EstimatedMonthCost { get; set; }
    }
}
