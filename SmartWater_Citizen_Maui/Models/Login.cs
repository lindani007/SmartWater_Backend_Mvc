using System;
using System.Collections.Generic;
using System.Text;

namespace SmartWater_Citizen_Maui.Models
{
    public class Login
    {
        public int Id { get; set; }
        public string? Account_Number { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? FullNames { get; set; }
        public string? Role { get; set; }
    }
}
