using System.ComponentModel.DataAnnotations;

namespace Smart_Water_Consumption_Api.Models
{
    public class RegionPrediction
    {
        [Key]
        public int Id { get; set; }
        public int RegionId { get; set; }
        public string RegionName { get; set; } = string.Empty;
        public double ActualConsumptionKl { get; set; }
        public double PredictedConsumptionKl { get; set; }
        public double EstimatedCost { get; set;  }
        public DateTime MonthYear { get; set; }
    }
}
