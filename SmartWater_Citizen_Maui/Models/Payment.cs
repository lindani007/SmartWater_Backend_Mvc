using System;
using System.Collections.Generic;
using System.Text;

namespace SmartWater_Citizen_Maui.Models
{
    public class Payment
    {
        public string? Account_Number { get; set; }
        public int Id { get; set; }
        public double Amount_Due { get; set; }
        public double Amount_Payed { get; set; }
        public double Outstanding_Due { get; set; }
        public DateTime PAYED_On { get; set; }
    }
}
