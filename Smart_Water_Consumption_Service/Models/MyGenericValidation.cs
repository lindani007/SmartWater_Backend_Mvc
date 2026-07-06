using System;
using System.Collections.Generic;
using System.Text;

namespace Smart_Water_Consumption_Service.Models
{
    public class MyGenericValidation<T>
    {
        public T? Data { get; set; }
        public string? Message { get; set; }
        public bool IsSuccess { get; set; }
    }
}
