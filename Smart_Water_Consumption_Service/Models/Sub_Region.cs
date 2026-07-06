using System.ComponentModel.DataAnnotations;

namespace Smart_Water_Consumption_Service.Models
{
    public class Sub_Region
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string SubRegionName { get; set; } = string.Empty;

        public int? RegionId { get; set; }
        [Required]
        [StringLength(100)]
        public string RegionName { get; set; } = string.Empty;
        public string SubRegion_Meter { get; set; } = string.Empty;
    }
}
