using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Smart_Water_Consumption_Service.Models
{
    public class ModelTrainingPayload
    {
            [Key]
            public int Id { get; set; }
            public int ApplianceActive_Id { get; set; }
            public string Account { get; set; } = string.Empty;
            public string Appliance_Name { get; set; } = string.Empty;
            public double Duration { get; set; }
            public int TimeOfTheDay { get; set; }
            public double DayUsage { get; set; }
            public double NightUsage { get; set; }
            public double WaterUsed { get; set; }
            public int Holiday { get; set; }
            public int PreviousLeak { get; set; }
            public double Last7Avearage { get; set; }
            public double Last30Avearage { get; set; }
            public int NumberOfHighUsages { get; set; }
            public double Flowrate { get; set; }
            public int DayOfWeek { get; set; }
            public int NumberOfPeople { get; set; }
            public int Status { get; set; }
            public double LeakageProbability { get; set; }
    }
}
