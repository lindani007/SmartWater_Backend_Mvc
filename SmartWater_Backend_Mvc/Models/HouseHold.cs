namespace SmartWater_Backend_Mvc.Models
{
    public class HouseHold
    {
        public int Household_Id { get; set; }
        public string? Account_Number { get; set; }
        public int SubRegion_Id { get; set; }
        public string? SubRegion_Name { get; set; }
        public double Water_Received { get; set; }
        public double Cost { get; set; }
        public string? Status { get; set; }
        public double Other_OutStandingCost { get; set; }
        public double? Night_Usage { get; set; }
        public double? Daily_Usage { get; set; }
        public double? Spike_Usage { get; set; }
        public double Leakage_Propability { get; set; }
        public double? FreeWater { get; set; }
        public DateTime Consumption_Month { get; set; }
    }
}
