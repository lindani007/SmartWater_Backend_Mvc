namespace Smart_Water_Consumption_Api.Models
{
    public class FuturePredictionRequest
    {
        public int ApplianceId { get; set; }

        public int HourOfDay { get; set; }

        public int DayOfWeek { get; set; }

        public int WeekOfYear { get; set; }

        public int Month { get; set; }

        public int Holiday { get; set; }

        public int NumberOfPeople { get; set; }

        public double FlowRate { get; set; }

        public double PreviousHourUsage { get; set; }

        public double YesterdayUsage { get; set; }

        public double Last7Average { get; set; }

        public double Last30Average { get; set; }

        public double CurrentWeekUsage { get; set; }

        public double CurrentMonthUsage { get; set; }
    }
}
