using System.ComponentModel.DataAnnotations;

namespace Smart_Water_Consumption_Api.Models
{
    public class Region
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Region_Name { get; set; } = string.Empty;
        public string? Region_Meter { get; set; }
    }
}
