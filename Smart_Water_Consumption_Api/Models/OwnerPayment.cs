using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Smart_Water_Consumption_Api.Models
{
    public class OwnerPayment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Account_Number { get; set; } = string.Empty;

        public double Amount_Payed { get; set; }

        public double Amount_Outstanding { get; set; }

        public double Amount_Owing { get; set; }

        public DateTime? Date { get; set; } = DateTime.UtcNow;
    }
}
