using System.ComponentModel.DataAnnotations;

namespace SmartWater_Backend_Mvc.Models
{
    public class Sub_Region
    {
        public int Id { get; set; }
        public int RegionId { get; set; }
        public string? SubRegionName { get; set; }
        [Required]
        [StringLength(10)]
        public string? Meter_Number { get; set; }
        public string? Region_Name { get; set; }
    }
}
