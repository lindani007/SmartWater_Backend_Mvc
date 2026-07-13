using System;
using System.Collections.Generic;
using System.Text;

namespace Smart_Water_Consumption_Service.Services.AiService.MachineLearningModel
{
    public class MachineLearningService
    {
        //private readonly InferenceSession _statusModel;
        //private readonly InferenceSession _nextHourModel;
        //private readonly InferenceSession _nextWeekModel;
        //private readonly InferenceSession _nextMonthModel;
        //public MachineLearningService()
        //{
        //    _statusModel = LoadModel("consumptionstatus_model.onnx");
        //    _nextHourModel = LoadModel("next_hour_brain.onnx");
        //    _nextWeekModel = LoadModel("next_week_brain.onnx");
        //    _nextMonthModel = LoadModel("next_month_brain.onnx");
        //}

        //private InferenceSession LoadModel(string modelName)
        //{
        //    string path = Path.Combine(
        //        AppDomain.CurrentDomain.BaseDirectory,
        //        "Services",
        //        "MachineLearningService",
        //        modelName);

        //    if (!File.Exists(path))
        //        throw new FileNotFoundException($"Model not found: {path}");

        //    return new InferenceSession(path);
        //}
        public int PredictConsumptionStatus()
        {
            return 0;
        }

        public double PredictNextHour()
        {
            return 0;
        }

        public double PredictNextWeek()
        {
            return 0;
        }

        public double PredictNextMonth()
        {
            return 0;
        }
    }
}
