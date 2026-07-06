using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Smart_Water_Consumption_Service.Models;
using Smart_Water_Consumption_Service.Services.DbServices;
using Smart_Water_Consumption_Service.Services.SignalRService;
using Smart_Water_Consumption_Service.Services.SimulationService;
using SmartWater_Citizen_Maui.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SmartWater_Citizen_Maui.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly ApplianceConsumptionDbService _applianceConsumptionDbService;
        private RealtimeDashboard _realtimeDashboard;
        public DashboardViewModel(ApplianceConsumptionDbService applianceConsumptionDbService) 
        {
            _applianceConsumptionDbService = applianceConsumptionDbService;
            _realtimeDashboard = new RealtimeDashboard();
            StartConnection();
        }
        [ObservableProperty]
        private double currentWaterUsed;
        [ObservableProperty]
        private double balanceToSettled;
        public async void StartConnection()
        {
            await _realtimeDashboard.StartConnection();
            _realtimeDashboard.OnDataReceived -= UpdateWaterConsumption;
           _realtimeDashboard.OnDataReceived += UpdateWaterConsumption;
        }

        [ObservableProperty]
        public ObservableCollection<ApplienceCosumption> applienceCosumptions = new ObservableCollection<ApplienceCosumption>();

        public async Task DisplayAppliences(string account)
        {
            Preferences.Default.Set("Account", account);
            var rawDate = await _applianceConsumptionDbService.GetAllApplianceConsumptions();
            if (rawDate?.Data != null)
            {
                List<ApplienceCosumption> applienceCosumptions = rawDate.Data.Select(x => new ApplienceCosumption
                {
                    Appliance_Id = x.Appliance_Id,
                    Appliance_Name = x.Appliance_Name,
                    Applience_Amount_Spent = x.Cost.Value,
                    Applience_Water_Consumed = (x.Daily_Usage.Value + x.Night_Usage.Value),
                    Status_Of_Spending = x.Daily_Status,
                    Account_Number = x.Account,
                    Spending_Date = x.Date.Value
                }).Where(x => x.Account_Number == account && x.Spending_Date.Date == DateTime.Today).ToList();

                await LoadData(applienceCosumptions);
            }
        }

        public async Task LoadData(List<ApplienceCosumption> applienceCosumptions)
        {
            var groups = applienceCosumptions.GroupBy(x => x.Appliance_Id);
            var newappliances = new List<ApplienceCosumption>();
            foreach (var group in groups)
            {
                newappliances.Add(new ApplienceCosumption
                {
                    Appliance_Id = group.Key,
                    Appliance_Name = group.FirstOrDefault()?.Appliance_Name,
                    Applience_Amount_Spent = group.Sum(x => x.Applience_Amount_Spent),
                    Applience_Water_Consumed = group.Sum(x => x.Applience_Water_Consumed),
                    Status_Of_Spending = group.FirstOrDefault()?.Status_Of_Spending,
                    Account_Number = group.FirstOrDefault()?.Account_Number,
                    Spending_Date = group.FirstOrDefault().Spending_Date
                });
            }

            ApplienceCosumptions = new ObservableCollection<ApplienceCosumption>(newappliances);
            CurrentWaterUsed = newappliances.Sum(x => x.Applience_Water_Consumed);
            BalanceToSettled = newappliances.Sum(x => x.Applience_Amount_Spent);
        }

        private async void UpdateWaterConsumption(bool fecthData)
        { 
            string account = Preferences.Default.Get("Account", string.Empty);
            if(fecthData)
            {
                var list = await GetDbAppliances();
                list = list.Where(x => x.Spending_Date.Date == DateTime.Today && x.Account_Number == account).ToList() ?? new List<ApplienceCosumption>();
                await LoadData(list);
            }
        }

        private async Task<List<Models.ApplienceCosumption>> GetDbAppliances()
        {
            var response = await _applianceConsumptionDbService.GetAllApplianceConsumptions();
            var ownerAppliances = response.Data.Select(x => new Models.ApplienceCosumption
            {
                Appliance_Id = x.Appliance_Id,
                Appliance_Name = x.Appliance_Name,
                Applience_Amount_Spent = x.Cost.Value,
                Applience_Water_Consumed = Math.Round((x.Daily_Usage.Value + x.Night_Usage.Value), 2),
                Status_Of_Spending = x.Daily_Status,
                Account_Number = x.Account,
                Spending_Date = x.Date.Value
            }).Where(x => x.Spending_Date.Date.Month == DateTime.Now.Month && x.Spending_Date.Year == DateTime.Now.Year).ToList();
            return ownerAppliances;
        }
    }    
}
