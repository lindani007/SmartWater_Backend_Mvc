
using Microsoft.Extensions.DependencyInjection;
using Smart_Water_Consumption_Service.Models;
using Smart_Water_Consumption_Service.Services.DbServices;
using SmartWater_Citizen_Maui;

namespace SmartWater_Citizen_Maui
{
    public partial class App : Application
    {
        private readonly ApiBaseUrl _apiBaseUrl;
        private readonly OwnerDbService _ownerDbService;
        private readonly LoginDbService _loginDbService;
        private readonly ApplianceDbService _applianceDbService;
        private readonly ApplianceConsumptionDbService _applianceConsumptionDbService;
        private readonly OwnerPaymentDbService _ownerPaymentDbService;
        public App()
        {
            InitializeComponent();
            _apiBaseUrl = new ApiBaseUrl { ApiUrl = "https://localhost:7203" };
            _loginDbService = new LoginDbService(_apiBaseUrl);
            _applianceConsumptionDbService = new ApplianceConsumptionDbService(_apiBaseUrl);
            _applianceDbService = new ApplianceDbService(_apiBaseUrl);
            _ownerDbService = new OwnerDbService(_apiBaseUrl);
            _ownerPaymentDbService = new OwnerPaymentDbService(_apiBaseUrl);
        }

        
        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new NavigationPage(new Views.Login(_loginDbService, _ownerDbService, _applianceDbService, _applianceConsumptionDbService, _ownerPaymentDbService)));
        }
    }
}