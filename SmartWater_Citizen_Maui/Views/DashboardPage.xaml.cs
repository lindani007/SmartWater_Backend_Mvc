using Microsoft.Maui.Controls;
using Smart_Water_Consumption_Service.Services.DbServices;
using Smart_Water_Consumption_Service.Services.SignalRService;
using Smart_Water_Consumption_Service.Services.SimulationService;
using SmartWater_Citizen_Maui.ViewModels;
using System;
using System.Threading.Tasks;

namespace SmartWater_Citizen_Maui.Views
{
    public partial class DashboardPage : ContentPage
    {
        DashboardViewModel _viewModel;
        string _account;
        ApplianceConsumptionDbService _applianceConsumptionDbService;

        public DashboardPage(string acc, ApplianceConsumptionDbService applianceConsumptionDbService)
        {
            InitializeComponent();

            _applianceConsumptionDbService = applianceConsumptionDbService;
            _viewModel = new DashboardViewModel(applianceConsumptionDbService);
            _account = acc;
            BindingContext = _viewModel;
            Loaded += DashboardPage_Loaded;
        }

        private async void DashboardPage_Loaded(object sender, EventArgs e)
        {
            try
            {
                _viewModel.StartConnection();

                await _viewModel.DisplayAppliences(_account);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during initialization: {ex.Message}");
            }
        }
    }
}
