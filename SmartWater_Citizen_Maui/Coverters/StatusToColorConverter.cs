using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SmartWater_Citizen_Maui.Coverters
{
    public class StatusToColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // Because it's a MultiValueConverter, 'values' is an array of objects
            if (values != null && values.Length > 0 && values[0] is string status)
            {
                if (status == "Active") return Colors.Green;
                if (status == "Warning") return Colors.Orange;
            }

            return Colors.Gray;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
