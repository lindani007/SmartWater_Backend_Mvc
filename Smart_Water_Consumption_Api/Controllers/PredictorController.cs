using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Smart_Water_Consumption_Api.Ai.MachineLearningModel;
using Smart_Water_Consumption_Api.Models;
using Smart_Water_Consumption_Api.Models.Db;

namespace Smart_Water_Consumption_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PredictorController : ControllerBase
    {
        private ModelsPrediction _modelsPrediction;
        public PredictorController(ModelsPrediction modelsPrediction    )
        {
            _modelsPrediction = modelsPrediction;
        }

        [HttpPost("status")]
        public int PredictStatus(ConsumptionStatusRequest consumptionStatusRequest)
        {
            int prediction = _modelsPrediction.PredictConsumptionStatus(consumptionStatusRequest.ApplianceId,
                    consumptionStatusRequest.Duration,
                    consumptionStatusRequest.FlowRate,
                    consumptionStatusRequest.WaterUsed,
                    consumptionStatusRequest.DayUsage,
                    consumptionStatusRequest.NightUsage,
                    consumptionStatusRequest.Holiday,
                    consumptionStatusRequest.PreviousLeak,
                    consumptionStatusRequest.Last7Average,
                    consumptionStatusRequest.Last30Average,
                    consumptionStatusRequest.NumberOfHighUsages,
                    consumptionStatusRequest.DayOfWeek,
                    consumptionStatusRequest.TimeOfTheDay,
                    consumptionStatusRequest.NumberOfPeople);
            return prediction;
        }

        [HttpPost("hour")]
        public double PredictFutureHourUsage(FuturePredictionRequest futurePredictionRequest)
        {
            double nextHourPrediction = _modelsPrediction.PredictNextHour(
                futurePredictionRequest.ApplianceId,
                futurePredictionRequest.HourOfDay,
                futurePredictionRequest.DayOfWeek,
                futurePredictionRequest.WeekOfYear,
                futurePredictionRequest.Month,
                futurePredictionRequest.Holiday,
                futurePredictionRequest.NumberOfPeople,
                futurePredictionRequest.FlowRate,
                futurePredictionRequest.PreviousHourUsage,
                futurePredictionRequest.YesterdayUsage,
                futurePredictionRequest.Last7Average,
                futurePredictionRequest.Last30Average,
                futurePredictionRequest.CurrentWeekUsage,
                futurePredictionRequest.CurrentMonthUsage
            );

            return nextHourPrediction;
        }

        [HttpPost("week")]
        public double PredictFutureWeekUsage(FuturePredictionRequest futurePredictionRequest)
        {
            double nextWeekPrediction = _modelsPrediction.PredictNextWeek(
                futurePredictionRequest.ApplianceId,
                futurePredictionRequest.HourOfDay,
                futurePredictionRequest.DayOfWeek,
                futurePredictionRequest.WeekOfYear,
                futurePredictionRequest.Month,
                futurePredictionRequest.Holiday,
                futurePredictionRequest.NumberOfPeople,
                futurePredictionRequest.FlowRate,
                futurePredictionRequest.PreviousHourUsage,
                futurePredictionRequest.YesterdayUsage,
                futurePredictionRequest.Last7Average,
                futurePredictionRequest.Last30Average,
                futurePredictionRequest.CurrentWeekUsage,
                futurePredictionRequest.CurrentMonthUsage
            );

            return nextWeekPrediction;
        }
        [HttpPost("month")]
        public double PredictFutureMonthkUsage(FuturePredictionRequest futurePredictionRequest)
        {
            double nextMonthPrediction = _modelsPrediction.PredictNextMonth(
                futurePredictionRequest.ApplianceId,
                futurePredictionRequest.HourOfDay,
                futurePredictionRequest.DayOfWeek,
                futurePredictionRequest.WeekOfYear,
                futurePredictionRequest.Month,
                futurePredictionRequest.Holiday,
                futurePredictionRequest.NumberOfPeople,
                futurePredictionRequest.FlowRate,
                futurePredictionRequest.PreviousHourUsage,
                futurePredictionRequest.YesterdayUsage,
                futurePredictionRequest.Last7Average,
                futurePredictionRequest.Last30Average,
                futurePredictionRequest.CurrentWeekUsage,
                futurePredictionRequest.CurrentMonthUsage
            );

            return nextMonthPrediction;
        }
    }
}
