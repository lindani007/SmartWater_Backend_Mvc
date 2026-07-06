using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Smart_Water_Consumption_Service.Models
{
    public class SubRegionPrediction
    {
        [Key]
        public int Id { get; set; }
        public int SubRegionId { get; set; }
        public string SubRegionName { get; set; } = string.Empty;
        public double ActualConsumptionKl { get; set; }
        public double PredictedConsumptionKl { get; set; }
        public double EstimatedCost { get; set; }
        public DateTime MonthYear { get; set; }
    }
}
