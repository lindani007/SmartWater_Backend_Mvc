using System;
using System.Collections.Generic;
using System.Text;

namespace SmartWater_Citizen_Maui.Models
{
    public class Appliance
    {
       public int Appliance_Id { get; set; }
       public string? Appliance_Name { get; set; } 
        public string? Account_Number { get; set; } 
       public string? Metre_Number { get; set; }
        public int PeopleUsingIt { get; set; }
    }
}
