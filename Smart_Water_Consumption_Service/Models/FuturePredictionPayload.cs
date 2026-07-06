using System;
using System.Collections.Generic;
using System.Text;

namespace Smart_Water_Consumption_Service.Models
{
    public class FuturePredictionPayload
    {
        public string Appliance_Name { get; set; } = string.Empty;
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
        public double NextHourUsage { get; set; }
        public double NextWeekUsage { get; set; }
        public double NextMonthUsage { get; set; }
    }
}
