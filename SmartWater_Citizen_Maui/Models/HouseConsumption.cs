using System;
using System.Collections.Generic;
using System.Text;

namespace SmartWater_Citizen_Maui.Models
{
    public class HouseConsumption
    {
        public string? Owner_Id { get; set; }
        public int House_Id { get; set; }
        public double Water_Received { get; set; }
        public double Total_WaterUsed { get; set; }
        public double Leakage_Propability { get; set; }
        public DateTime Month_OfConsumption { get; set; }
    }
}
