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
        public string AccountNumber { get; set; } = string.Empty;
        public double ActualConsumptionKl { get; set; }
        public double PredictedConsumptionKl { get; set; }
        public double EstimatedCost { get; set; }
        public DateTime MonthYear { get; set; }
    }
}
