using System;
using System.Collections.Generic;
using System.Text;

namespace SmartWater_Citizen_Maui.Models
{
    public class ApplienceCosumption
    {
        public int Appliance_Id { get; set; }
        public string? Appliance_Name { get; set; }
        public double Applience_Amount_Spent { get; set; }
        public double Applience_Water_Consumed { get; set; }
        public string? Status_Of_Spending { get; set; }
        public string? Account_Number { get; set; }
        public DateTime Spending_Date { get; set; }
        public double Leakage_Percent { get; set; }
    }
}
