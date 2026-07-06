namespace SmartWater_Backend_Mvc.Models
{
    public class RegionConsumption
    {
        public int Id { get; set; }
        public int Region_Id { get; set; }
        public string? Region_Name { get; set; } 
        public double Water_Used { get; set; }
        public double Water_Sent { get; set; }
        public double Water_Thats_OutStanding { get; set; }
        public double Expected_Cost { get; set; }
        public double Actual_Cost { get; set; }
        public double OutStanding_Cost { get; set; }
        public double? Night_Usage { get; set; }
        public double? Daily_Usage { get; set; }
        public double? Spike_Usage { get; set; }
        public double? FreeWater { get; set; }
        public double Leakage_Propability { get; set; }
        public DateTime Consumption_Month { get; set; }
    }
}
