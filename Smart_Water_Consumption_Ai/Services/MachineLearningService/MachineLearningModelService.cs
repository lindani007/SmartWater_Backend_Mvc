using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Timers;

namespace Smart_Water_Consumption_Ai.Services.MachineLearningService
{
    public class MachineLearningModelService
    {

        // Morning = 0, Afternoon = 1, Evening = 2, Night = 3
        private readonly InferenceSession _statusModel;
        private readonly InferenceSession _nextHourModel;
        private readonly InferenceSession _nextWeekModel;
        private readonly InferenceSession _nextMonthModel;
        public MachineLearningModelService()
        {
            _statusModel = LoadModel("consumptionstatus_model.onnx");
            _nextHourModel = LoadModel("next_hour_brain.onnx");
            _nextWeekModel = LoadModel("next_week_brain.onnx");
            _nextMonthModel = LoadModel("next_month_brain.onnx");
        }

        private InferenceSession LoadModel(string modelName)
        {
            string path = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Services",
                "MachineLearningService",
                modelName);

            if (!File.Exists(path))
                throw new FileNotFoundException($"Model not found: {path}");

            return new InferenceSession(path);
        }
        public int PredictConsumptionStatus(int applianceName, double duration, double flowRate,
        double waterUsed, double dayUsage, double nightUsage,
        int holiday, int previousLeak, double last7Average, double last30Average,
        int numberOfHighUsages, int dayOfWeek, int timeOfTheDay, int numberOfPeople)
        {
            float[] input =
            {
                (float)applianceName,
                (float)duration,
                (float)flowRate,
                (float)waterUsed,
                (float)dayUsage,
                (float)nightUsage,
                (float)holiday,
                (float)previousLeak,
                (float)last7Average,
                (float)last30Average,
                (float)numberOfHighUsages,
                (float)dayOfWeek,
                (float)timeOfTheDay,
                (float)numberOfPeople
            };
            var tensor = new DenseTensor<float>( input, new[] { 1, 14 });
            var inputs = new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor("float_input", tensor) };
            using IDisposableReadOnlyCollection<DisposableNamedOnnxValue> results = _statusModel.Run(inputs);
            var output = results.FirstOrDefault();
            if (output == null)
                throw new Exception("No prediction returned.");
            int prediction = (int)output.AsTensor<float>()[0];
            Debug.WriteLine(prediction);
            return prediction;
        }

        public double PredictNextHour(int applianceId, int hourOfDay, int dayOfWeek,
        int weekOfYear, int month, int holiday, int numberOfPeople, double flowRate,
        double previousHourUsage, double yesterdayUsage, double last7Average,
        double last30Average, double currentWeekUsage, double currentMonthUsage)
        {
            float[] input =
            {
                (float)applianceId,
                (float)hourOfDay,
                (float)dayOfWeek,
                (float)weekOfYear,
                (float)month,
                (float)holiday,
                (float)numberOfPeople,
                (float)flowRate,
                (float)previousHourUsage,
                (float)yesterdayUsage,
                (float)last7Average,
                (float)last30Average,
                (float)currentWeekUsage,
                (float)currentMonthUsage
            };
            var tensor = new DenseTensor<float>(input, new[] { 1, 14 });
            var inputs = new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor("float_input", tensor) };
            using IDisposableReadOnlyCollection<DisposableNamedOnnxValue> results = _nextHourModel.Run(inputs);
            var output = results.FirstOrDefault();
            if (output == null)
                throw new Exception("No prediction returned.");
            double prediction = (double)output.AsTensor<float>()[0];
            Debug.WriteLine(prediction);
            return prediction;
        }

        public double PredictNextWeek(int applianceId, int hourOfDay, int dayOfWeek,
        int weekOfYear, int month, int holiday, int numberOfPeople, double flowRate,
        double previousHourUsage, double yesterdayUsage, double last7Average,
        double last30Average, double currentWeekUsage, double currentMonthUsage)
        {
            float[] input =
            {
                (float)applianceId,
                (float)hourOfDay,
                (float)dayOfWeek,
                (float)weekOfYear,
                (float)month,
                (float)holiday,
                (float)numberOfPeople,
                (float)flowRate,
                (float)previousHourUsage,
                (float)yesterdayUsage,
                (float)last7Average,
                (float)last30Average,
                (float)currentWeekUsage,
                (float)currentMonthUsage
            };
            var tensor = new DenseTensor<float>(input, new[] { 1, 14 });
            var inputs = new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor("float_input", tensor) };
            using IDisposableReadOnlyCollection<DisposableNamedOnnxValue> results = _nextWeekModel.Run(inputs);
            var output = results.FirstOrDefault();
            if (output == null)
                throw new Exception("No prediction returned.");
            double prediction = (double)output.AsTensor<float>()[0];
            Debug.WriteLine(prediction);
            return prediction;
        }

        public double PredictNextMonth(int applianceId, int hourOfDay, int dayOfWeek,
        int weekOfYear, int month, int holiday, int numberOfPeople, double flowRate,
        double previousHourUsage, double yesterdayUsage, double last7Average,
        double last30Average, double currentWeekUsage, double currentMonthUsage)
        {
            float[] input =
            {
                (float)applianceId,
                (float)hourOfDay,
                (float)dayOfWeek,
                (float)weekOfYear,
                (float)month,
                (float)holiday,
                (float)numberOfPeople,
                (float)flowRate,
                (float)previousHourUsage,
                (float)yesterdayUsage,
                (float)last7Average,
                (float)last30Average,
                (float)currentWeekUsage,
                (float)currentMonthUsage
            };
            var tensor = new DenseTensor<float>(input, new[] { 1, 14 });
            var inputs = new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor("float_input", tensor) };
            using IDisposableReadOnlyCollection<DisposableNamedOnnxValue> results = _nextMonthModel.Run(inputs);
            var output = results.FirstOrDefault();
            if (output == null)
                throw new Exception("No prediction returned.");
            double prediction = (double)output.AsTensor<float>()[0];
            Debug.WriteLine(prediction);
            return prediction;
        }
    }
}
