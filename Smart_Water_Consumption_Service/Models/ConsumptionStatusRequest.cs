using System;
using System.Collections.Generic;
using System.Text;

namespace Smart_Water_Consumption_Service.Models
{
    public class ConsumptionStatusRequest
    {
        public int ApplianceId { get; set; }

        public double Duration { get; set; }

        public double FlowRate { get; set; }

        public double WaterUsed { get; set; }

        public double DayUsage { get; set; }

        public double NightUsage { get; set; }

        public int Holiday { get; set; }

        public int PreviousLeak { get; set; }

        public double Last7Average { get; set; }

        public double Last30Average { get; set; }

        public int NumberOfHighUsages { get; set; }

        public int DayOfWeek { get; set; }

        // Morning = 0, Afternoon = 1, Evening = 2, Night = 3
        public int TimeOfTheDay { get; set; }

        public int NumberOfPeople { get; set; }
    }
}
