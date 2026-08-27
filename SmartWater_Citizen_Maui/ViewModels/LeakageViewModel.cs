using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microcharts;
using Org.BouncyCastle.Tsp;
using SkiaSharp;
using Smart_Water_Consumption_Service.Models;
using Smart_Water_Consumption_Service.Services.DbServices;
using SmartWater_Citizen_Maui.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SmartWater_Citizen_Maui.ViewModels;


public partial class LeakageViewModel : ObservableObject
{
    [ObservableProperty]
    private double _waterReceivedKl;
    [ObservableProperty]
    private double _totalWaterUsedKl;
    [ObservableProperty]
    private double _leakagePercent;
    [ObservableProperty]
    private double _goodWaterPercent;
    [ObservableProperty]
    private string _summaryText = string.Empty;
    [ObservableProperty]
    private double _predictedNextMonthTotalKl;
    [ObservableProperty]
    private double _predictedNextMonthCost;
    [ObservableProperty]
    private Chart _leakagePieChart;

    public ObservableCollection<ApplianceLeakageDisplay> ApplianceUsages { get; set; } = new();
    public ObservableCollection<AppliancePredictionDisplay> AppliancePredictions { get; set; } = new();
    public ObservableCollection<MonthComparisonDisplay> HistoricalComparisons { get; set; } = new();

    private const double EstimatedCostPerKl = 12.50;

    public async Task LoadLeakageData(string accountNumber, Models.HouseConsumption houseInfo, List<ApplienceCosumption> rawLogs)
    {
        if (houseInfo == null) return;

        WaterReceivedKl = houseInfo.Water_Received;
        TotalWaterUsedKl = houseInfo.Total_WaterUsed;
        LeakagePercent = houseInfo.Leakage_Propability;
        GoodWaterPercent = 100.0 - LeakagePercent;

        SummaryText = $"Your house received {WaterReceivedKl:N2}kL of water, which is 100%. " +
                      $"Your house appliances used {TotalWaterUsedKl:N2}kL of water which is {((TotalWaterUsedKl / WaterReceivedKl) * 100):N0}% " +
                      $"and your suspected water loss or leakage is {LeakagePercent:N0}%. " +
                      $"Please follow up your water usage so that you know each and every drop of it.";

        MainThread.BeginInvokeOnMainThread(() =>
        {
            ApplianceUsages.Clear();
            AppliancePredictions.Clear();
            HistoricalComparisons.Clear();
        });

        var chartEntries = new List<ChartEntry>
        {
            new ChartEntry((float)GoodWaterPercent)
            {
                Label = "Known Usage",
                ValueLabel = $"{GoodWaterPercent:N0}%",
                Color = SKColor.Parse("#29ede7")
            },
            new ChartEntry((float)LeakagePercent)
            {
                Label = "Leakage/ Unknown Usage",
                ValueLabel = $"{LeakagePercent:N0}%",
                Color = SKColor.Parse("#FF0000")
            }
        };

        LeakagePieChart = new PieChart
        {
            Entries = chartEntries,
            LabelTextSize = 20f,
            BackgroundColor = SKColors.Transparent,
            HoleRadius = 0f,
            LabelColor = SKColors.White
        };

        if (rawLogs == null || !rawLogs.Any()) return;
        var userLogs = rawLogs.Where(x => x.Account_Number == accountNumber).ToList();

        if (!userLogs.Any()) return;

        var applianceGroups = userLogs.GroupBy(x => x.Appliance_Name);
        double totalReceivedLiters = WaterReceivedKl * 1000.0;

        var localUsages = new List<ApplianceLeakageDisplay>();

        foreach (var group in applianceGroups)
        {
            double appkl = group.Sum(x => x.Applience_Water_Consumed) / 1000;
            appkl = Math.Round(appkl, 2);
            double appLiters = group.Sum(x => x.Applience_Water_Consumed);
            double appPercentOfReceived = WaterReceivedKl > 0 ? (appkl / WaterReceivedKl) * 100.0 : 0;
            double avgLeakageProp = group.Any() ? group.Average(x => x.Leakage_Percent) : 0;

            localUsages.Add(new ApplianceLeakageDisplay
            {
                ApplianceName = group.Key ?? "Unknown",
                WaterUsedLiters = appLiters,
                PercentOfTotalReceived = appPercentOfReceived,
                LeakagePercent = avgLeakageProp
            });
        }

        int count = applianceGroups.Count();
        var localPredictions = await ReturnApplieancePredictions(accountNumber, count);
        var localHistoryAndPredictions = await ReturnPredictions(accountNumber);
        MainThread.BeginInvokeOnMainThread(() =>
        {
            foreach (var item in localUsages)
            {
                ApplianceUsages.Add(item);
            }

            foreach (var pred in localPredictions)
            {
                AppliancePredictions.Add(pred);
            }

            foreach (var record in localHistoryAndPredictions)
            {
                HistoricalComparisons.Add(record);
            }
        });
    }

    private async Task<List<MonthComparisonDisplay>> ReturnPredictions(string acc)
    {
        ApiBaseUrl apiBaseUrl = new ApiBaseUrl() { ApiUrl = "https://localhost:7203" };
        HouseholdPredictionDbService householdPrediction = new HouseholdPredictionDbService(apiBaseUrl);
        var rawData = await householdPrediction.GetAllHouseholdPredictions();

        if (rawData?.Data == null) return new List<MonthComparisonDisplay>();

        List<MonthComparisonDisplay> oldpredicts = rawData.Data
            .Where(x => x.AccountNumber == acc)
            .Select(x => new MonthComparisonDisplay
            {
                MonthYear = x.MonthYear.ToString("MMMM yyyy"),
                PredictedConsumptionKl = x.PredictedMonthConsumptionKl,
                ActualMonthConsumptionKl = 0,
                EstimatedCost = x.EstimatedMonthCost,
                Account = x.AccountNumber,
                PredictedHourConsumptionKl = x.PredictedHourConsumptionKl,
                PredictedWeekConsumptionKl = x.PredictedWeekConsumptionKl,
                PredictedMonthConsumptionKl = x.PredictedMonthConsumptionKl,
                EstimatedHourCost = x.EstimatedHourCost,
                EstimatedWeekCost = x.EstimatedWeekCost,
                EstimatedMonthCost = x.EstimatedMonthCost
            }).ToList();

        return oldpredicts;
    }

    private async Task<List<AppliancePredictionDisplay>> ReturnApplieancePredictions(string acc, int appliancesCount)
    {
        ApiBaseUrl apiBaseUrl = new ApiBaseUrl() { ApiUrl = "https://localhost:7203" };
        AppliancePredictionDbService appliancePredictionDbService = new AppliancePredictionDbService(apiBaseUrl);
        var response = await appliancePredictionDbService.GetAllAppliancePredictions();

        if (response?.Data == null) return new List<AppliancePredictionDisplay>();

        var filteredData = response.Data.Where(x => x.Owner_AccountNumber == acc).OrderByDescending(x => x.MonthYear).Take(appliancesCount).ToList();

        List<AppliancePredictionDisplay> appliancePredictionDisplays = filteredData.Select(x => new AppliancePredictionDisplay
        {
            ApplianceName = x.Appliance_Name,
            PredictedCost = Math.Max(0, x.EstimatedMonthCost),
            PredictedWaterKl = Math.Round(x.PredictedMonthConsumptionKl / 1000, 2),
            PredictedHourConsumptionKl = x.PredictedHourConsumptionKl,
            PredictedWeekConsumptionKl = x.PredictedWeekConsumptionKl,
            PredictedMonthConsumptionKl = x.PredictedMonthConsumptionKl,
            EstimatedHourCost = Math.Max(0, x.EstimatedHourCost),
            EstimatedWeekCost = Math.Max(0, x.EstimatedWeekCost),
            EstimatedMonthCost = Math.Max(0, x.EstimatedMonthCost)
        }).ToList();

        PredictedNextMonthTotalKl = Math.Round(appliancePredictionDisplays.Sum(x => x.PredictedWaterKl), 2);
        PredictedNextMonthCost = appliancePredictionDisplays.Sum(x => x.EstimatedMonthCost);

        return appliancePredictionDisplays;
    }
}

public class ApplianceLeakageDisplay
{
    public string ApplianceName { get; set; } = string.Empty;
    public double WaterUsedLiters { get; set; }
    public double PercentOfTotalReceived { get; set; }
    public double LeakagePercent { get; set; }
}

public class AppliancePredictionDisplay
{
    public string ApplianceName { get; set; } = string.Empty;
    public double PredictedWaterKl { get; set; }
    public double PredictedCost { get; set; }
    public double PredictedHourConsumptionKl { get; set; }
    public double PredictedWeekConsumptionKl { get; set; }
    public double ActualMonthConsumptionKl { get; set; }
    public double PredictedMonthConsumptionKl { get; set; }
    public double EstimatedHourCost { get; set; }
    public double EstimatedWeekCost { get; set; }
    public double EstimatedMonthCost { get; set; }
}

public class MonthComparisonDisplay
{
    public string MonthYear { get; set; } = string.Empty;
    public double PredictedHourConsumptionKl { get; set; }
    public double PredictedWeekConsumptionKl { get; set; }
    public double ActualMonthConsumptionKl { get; set; }
    public double PredictedMonthConsumptionKl { get; set; }
    public double EstimatedHourCost { get; set; }
    public double EstimatedWeekCost { get; set; }
    public double EstimatedMonthCost { get; set; }
    public double PredictedConsumptionKl { get; set; }
    public double EstimatedCost { get; set; }
    public string Account { get; set; } = string.Empty;
}