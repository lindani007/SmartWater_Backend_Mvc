using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Smart_Water_Consumption_Service.Models
{
    public class HouseholdPrediction
    {
        [Key]
        public int Id { get; set; }
        public int SubRegionId { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
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
