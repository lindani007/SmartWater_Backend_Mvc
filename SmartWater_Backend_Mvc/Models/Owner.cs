using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace SmartWater_Backend_Mvc.Models
{
    public class Owner
    {
        [StringLength(13)]
        public string? OwnerId_Number { get; set; }

        [Required]
        [MinLength(3)]
        public string? OwnerName { get; set; }
        public string? Account_Number { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [StringLength(10)]
        public string? Meter_Number { get; set; }

        [Required]
        [Phone]
        public string? Phone { get; set; }
        [Required]
        public string PropertyValue { get; set; } = string.Empty;

        [Required]
        public int HouseId { get; set; }
        public int SubRegionId { get; set; }

        [Required]
        [MinLength(4)]
        public string? SubRegionName { get; set; }
        public double OutstandingBalance { get; set; }
        public bool IsPaid => OutstandingBalance <= 0;
    }
}
