using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Smart_Water_Consumption_Service.Services.DbServices;
using Smart_Water_Consumption_Service.Services.SimulationService;
using SmartWater_Citizen_Maui.Models;
using SmartWater_Citizen_Maui.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;
using System.Runtime.CompilerServices;
using System.Text;

namespace SmartWater_Citizen_Maui.ViewModels
{
    public partial class ApplienceViewModel : ObservableObject
    {
        [ObservableProperty]
        private string applienceName;

        [ObservableProperty]
        private string meterNumber;

        [ObservableProperty]
        private string account;

        [ObservableProperty]
        private int peopleUsingIt;

        private int applienceId;

        [ObservableProperty]
        private ObservableCollection<Appliance> myAppliances = new ObservableCollection<Appliance>();

        private readonly ApplianceDbService _applianceDbService;
        private readonly ApplianceConsumptionDbService _applianceConsumptionDbService;
        private readonly OwnerPaymentDbService _ownerPaymentDbService;
        public ApplienceViewModel(string account, ApplianceDbService applianceDbService, 
            ApplianceConsumptionDbService applianceConsumptionDbService, OwnerPaymentDbService ownerPaymentDbService)
        {
            this.Account = account;
            _applianceDbService = applianceDbService;
            _applianceConsumptionDbService = applianceConsumptionDbService;
            _ownerPaymentDbService = ownerPaymentDbService;
            Task.Run(async () =>
            {
                MyAppliances = new ObservableCollection<Appliance>(await GetOwnerAppliences(account));
            });
        }

        [RelayCommand]
        private async Task AddClicked()
        {
            string results = await AddApplience(ApplienceName, MeterNumber, Account, PeopleUsingIt);
            if (results != "Applience added succesful")
            {
                await App.Current.MainPage.DisplayAlertAsync("Adding Failed", results, "Retry");
                return;
            }

            await App.Current.MainPage.DisplayAlertAsync("Adding Applience Succesful", results, "Okay");
            ApplienceName = string.Empty;
            MeterNumber = string.Empty;
            PeopleUsingIt = 0;
        }

        [RelayCommand]
        private async Task SaveClicked()
        {
            List<Appliance> appliaences = new List<Appliance>();
            string id = Preferences.Default.Get("ApplianceId", string.Empty);
            string results = await UpdateApplience(int.Parse(id),ApplienceName, MeterNumber, Account, PeopleUsingIt);
            if (results != "Applience added succesful")
            {
                await App.Current.MainPage.DisplayAlertAsync("Adding Failed", results, "Retry");
                return;
            }

            await App.Current.MainPage.DisplayAlertAsync("Editing Applience Succesful", "Appliance has been edited succesful.", "Okay");
            if (Application.Current?.MainPage is FlyoutPage flyoutPage && flyoutPage.Detail is NavigationPage navPage)
            {
                await navPage.PopAsync();
            }
            Preferences.Default.Remove("ApplianceId");
            ApplienceName = string.Empty;
            MeterNumber = string.Empty;
            PeopleUsingIt = 0;
        }

        [RelayCommand]
        private async Task AddMoreClicked()
        {
            
            if (Application.Current?.MainPage is FlyoutPage flyoutPage && flyoutPage.Detail is NavigationPage navPage)
            {
                await navPage.PushAsync(new AddAplienceView(Account, _applianceDbService, _applianceConsumptionDbService, _ownerPaymentDbService));
            }
        }

        [RelayCommand]
        private async Task DeleteClicked(Appliance appliance)
        {
            if (appliance == null)
                return;
            var response = await _applianceDbService.DeleteAppliance(appliance.Appliance_Id);
            if(response.IsSuccess)
            {
                MyAppliances.Remove(appliance);
                MyAppliances = new ObservableCollection<Appliance>(MyAppliances);
                return;
            }
           await App.Current.MainPage.DisplayAlertAsync("Failed To Delete Appliance", response.Message, "Retry");
            return;
        }

        [RelayCommand]
        private async Task EditClicked(Appliance appliance)
        {
            if (appliance == null) return;

            Account = appliance.Account_Number;
            applienceId = appliance.Appliance_Id;
            MeterNumber = appliance.Metre_Number;
            ApplienceName = appliance.Appliance_Name;
            PeopleUsingIt = appliance.PeopleUsingIt;
            Preferences.Default.Set("ApplianceId", applienceId);
            MyAppliances.Remove(appliance);
            MyAppliances = new ObservableCollection<Appliance>(MyAppliances);
            if (Application.Current?.MainPage is FlyoutPage flyoutPage && flyoutPage.Detail is NavigationPage navPage)
            {
                await navPage.PushAsync(new EditAppliance(this));
            }
        }

        [RelayCommand]
        private async Task DoneClicked()
        {
            bool hasAppliances = await DoesUserHasAddedAnyAppliances(Account);
            if(hasAppliances)
                 App.Current.MainPage = new Home(Account, _applianceDbService, _applianceConsumptionDbService,_ownerPaymentDbService);
            else
                await App.Current.MainPage.DisplayAlertAsync("Warning❗❗❗", "Please add at least one appliance to continue using the app", "Retry");
        }

        private async Task<string> AddApplience(string name, string metre, string account, int numPeople)
        {
            if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(metre))
                return "Applience name and meter it's using are required";
            else if (string.IsNullOrEmpty(name))
                return "Applience name is required";
            else if (name.Length < 3)
                return "Applience name must be atleast 3 charecters";
            else if (string.IsNullOrEmpty(metre))
                return "Meter number is required";
            else if (metre.Length < 10)
                return "Meter number must be atleast 10 charecters";
            else
            {
                var applianceToAdd = new Smart_Water_Consumption_Service.Models.Appliance
                {
                    Appliance_Name = name,
                    Appliance_MeterNumber = metre,
                    Owner_AccountNumber = account,
                    NumberOfPeopleUsingIt = numPeople
                };
                var response = await _applianceDbService.AddAppliance(applianceToAdd);
                if(response.IsSuccess)
                    return "Applience added succesful";

                return response.Message ?? string.Empty;
            }
        }
        private async Task<string> UpdateApplience(int id ,string name, string metre, string account, int numPeople)
        {
            if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(metre))
                return "Applience name and meter it's using are required";
            else if (string.IsNullOrEmpty(name))
                return "Applience name is required";
            else if (name.Length < 3)
                return "Applience name must be atleast 3 charecters";
            else if (string.IsNullOrEmpty(metre))
                return "Meter number is required";
            else if (metre.Length < 10)
                return "Meter number must be atleast 10 charecters";
            else
            {
                var applianceToAdd = new Smart_Water_Consumption_Service.Models.Appliance
                {
                    Appliance_Id = id,
                    Appliance_Name = name,
                    Appliance_MeterNumber = metre,
                    Owner_AccountNumber = account,
                    NumberOfPeopleUsingIt = numPeople
                };
                var response = await _applianceDbService.UpdateAppliance(applianceToAdd);

                if (response.IsSuccess)
                    return "Applience added succesful";

                return response.Message ?? string.Empty;
            }
        }
        private async Task<List<Appliance>> GetOwnerAppliences(string account)
        {
            var rawData = await _applianceDbService.GetAllAppliance();
            List<Appliance> appliences = rawData.Data.Select(x => new Models.Appliance
            {
                Appliance_Name = x.Appliance_Name,
                Metre_Number = x.Appliance_MeterNumber,
                Account_Number = x.Owner_AccountNumber,
                Appliance_Id = x.Appliance_Id,
                PeopleUsingIt = x.NumberOfPeopleUsingIt
            }).Where(x => x.Account_Number == account).ToList();
            return appliences;
        }

        private async Task<bool> DoesUserHasAddedAnyAppliances(string acc)
        {
            var appliances = await _applianceDbService.GetAllAppliance();
            return appliances.Data.Any(x => x.Owner_AccountNumber == acc);
        }

    }   
}
