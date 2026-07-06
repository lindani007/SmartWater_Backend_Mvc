using System.ComponentModel.DataAnnotations;

namespace SmartWater_Backend_Mvc.Models
{
    public class Region
    {
        public int Region_Id { get; set; }
        [Required]
        [MinLength(3)]
        public string? Region_Name { get; set; }
        [Required]
        [StringLength(10)]
        public string? Meter_Number { get; set; }
    }
}
