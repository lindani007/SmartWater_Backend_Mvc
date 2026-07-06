using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Smart_Water_Consumption_Service.Services.DbServices;
using SmartWater_Citizen_Maui.Models;
using SmartWater_Citizen_Maui.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace SmartWater_Citizen_Maui.ViewModels
{
    public partial class BillsViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<BillingHistorySummary> _pastBills = new();

        [ObservableProperty] private double _prevBalance;
        [ObservableProperty] private string _paymentsReceived;
        [ObservableProperty] private double _currentWaterCharges;
        [ObservableProperty] private double _totalDue;
        [ObservableProperty] private string _paymentDueDate = string.Empty;
        private readonly ApplianceConsumptionDbService _applianceConsumptionDbService;
        private readonly OwnerPaymentDbService _ownerPaymentDbService;
        public BillsViewModel(ApplianceConsumptionDbService applianceConsumptionDbService, OwnerPaymentDbService ownerPaymentDbService)
        {
            _applianceConsumptionDbService = applianceConsumptionDbService;
            _ownerPaymentDbService = ownerPaymentDbService;
        }

        public async Task LoadBillingData(string account)
        {
            var dataFromDb = await _applianceConsumptionDbService.GetAllApplianceConsumptions();
            List<ApplienceCosumption> userConsumption = dataFromDb.Data.Select(x => new ApplienceCosumption
            {
                Appliance_Id = x.Appliance_Id,
                Appliance_Name = x.Appliance_Name,
                Applience_Amount_Spent = x.Cost.Value,
                Applience_Water_Consumed = (x.Daily_Usage.Value + x.Night_Usage.Value),
                Status_Of_Spending = x.Daily_Status,
                Account_Number = x.Account,
                Spending_Date = x.Date.Value
            }).Where( x => x.Account_Number == account ).ToList();

            var rawDbPayments = await _ownerPaymentDbService.GetAllOwnersPayments();
            List<Payment> userPayments = rawDbPayments.Data.Select(x => new Payment
            {
                Account_Number = x.Account_Number,
                Id = x.Id,
                Amount_Due = x.Amount_Owing,
                Amount_Payed = x.Amount_Payed,
                Outstanding_Due = x.Amount_Outstanding,
                PAYED_On = x.Date.Value
            }).Where(x =>  x.Account_Number == account).ToList();

            DateTime currentDateTime = DateTime.Now;
            var groups = userConsumption.GroupBy(x => x.Appliance_Id);
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
            CurrentWaterCharges = newappliances
                .Where(x => x.Spending_Date.Month == currentDateTime.Month &&
                            x.Spending_Date.Year == currentDateTime.Year)
                .Sum(x => x.Applience_Amount_Spent);

            var historicalPayments = userPayments
                .Where(x => (x.PAYED_On.Month < currentDateTime.Month || x.PAYED_On.Year < currentDateTime.Year))
                .ToList();

            PrevBalance = historicalPayments.Sum(x => x.Amount_Due) - historicalPayments.Sum(x => x.Amount_Payed);
            PaymentsReceived = account;

            TotalDue = CurrentWaterCharges + PrevBalance;
            if (TotalDue < 0)
            {
                TotalDue = 0.00;
            }

            DateTime nextMonthFirstDay = new DateTime(currentDateTime.Year, currentDateTime.Month, 1).AddMonths(1);
            PaymentDueDate = nextMonthFirstDay.ToString("dd MMMM yyyy");

            PastBills.Clear();

            var monthlyGroups = newappliances
                .GroupBy(x => new { x.Spending_Date.Year, x.Spending_Date.Month })
                .OrderByDescending(g => g.Key.Year)
                .ThenByDescending(g => g.Key.Month);

            foreach (var group in monthlyGroups)
            {
                DateTime groupDate = new DateTime(group.Key.Year, group.Key.Month, 1);

                PastBills.Add(new BillingHistorySummary
                {
                    Billing_Month = groupDate.ToString("MMMM yyyy"),
                    Total_Water_Liters = group.Sum(x => x.Applience_Water_Consumed),
                    Total_Cost_Rands = group.Sum(x => x.Applience_Amount_Spent),
                    RawDailyLogs = group.ToList()
                });
            }
        }

        [RelayCommand]
        private async Task ViewHistoryDetails(BillingHistorySummary selectedSummary)
        {
            if (selectedSummary == null) return;
            if (Application.Current?.MainPage is FlyoutPage flyoutPage && flyoutPage.Detail is NavigationPage navPage)
            {
                await navPage.PushAsync(new MonthlyBills(selectedSummary));
            }
        }

        [RelayCommand]
        private async void PayClicked()
        {
            if (Application.Current?.MainPage is FlyoutPage flyoutPage && flyoutPage.Detail is NavigationPage navPage)
            {
                await navPage.PushAsync(new SettleBills());
            }
        }
    }

    public class BillingHistorySummary
    {
        public string Billing_Month { get; set; }
        public double Total_Water_Liters { get; set; }
        public double Total_Cost_Rands { get; set; }
        public List<ApplienceCosumption> RawDailyLogs { get; set; } = new();
    }
}
