using System;
using System.Collections.Generic;
using System.Text;

namespace Smart_Water_Consumption_Service.Services.TarrifService
{
    public class WaterConsumptionCalculation
    {
        public static double CalculateCostOfWater(double watersent, string propertyValue)
        {
            if (propertyValue == "Above250000")
                return TariffToUseForMiddleUpperClass(watersent);
            else if (propertyValue == "Under250000") 
                return TariffToUseForLowerClass(watersent);
            return -1;
        }

        private static double TariffToUseForMiddleUpperClass(double amountOfWater)
        {
            if (amountOfWater < 0) return 0;

            double totalCost = 0;

            if (amountOfWater > 6)
            {
                double blockWater = Math.Min(amountOfWater, 25) - 6;
                totalCost += blockWater * 31.85;
            }

            if (amountOfWater > 25)
            {
                double blockWater = Math.Min(amountOfWater, 30) - 25;
                totalCost += blockWater * 42.44;
            }

            if (amountOfWater > 30)
            {
                double blockWater = Math.Min(amountOfWater, 45) - 30;
                totalCost += blockWater * 65.45;
            }

            if (amountOfWater > 45)
            {
                double blockWater = amountOfWater - 45;
                totalCost += blockWater * 71.97;
            }

            return totalCost;
        }

        private static double TariffToUseForLowerClass(double amountOfWater)
        {
            if (amountOfWater < 0) return 0;

            double totalCost = 0;

            if (amountOfWater > 6)
            {
                double blockWater = Math.Min(amountOfWater, 25) - 6;
                totalCost += blockWater * 21.70;
            }

            if (amountOfWater > 25)
            {
                double blockWater = Math.Min(amountOfWater, 30) - 25;
                totalCost += blockWater * 29.68;
            }

            if (amountOfWater > 30)
            {
                double blockWater = Math.Min(amountOfWater, 45) - 30;
                totalCost += blockWater * 65.53;
            }

            if (amountOfWater > 45)
            {
                double blockWater = amountOfWater - 45;
                totalCost += blockWater * 71.97;
            }

            return totalCost;
        }
    }
}
