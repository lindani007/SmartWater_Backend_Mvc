using Smart_Water_Consumption_Service.Models;
using Smart_Water_Consumption_Service.Services.DbServices;
using SmartWater_Citizen_Maui.Models;

namespace SmartWater_Citizen_Maui.Views;

public partial class Home : FlyoutPage
{
    string _acc;
    ApplianceDbService _applianceDbService;
    ApplianceConsumptionDbService _applianceConsumptionDbService;
    private readonly OwnerPaymentDbService _ownerDbService;
    private readonly HouseConsumptionDbService _houseConsumptionDbService;

    public Home(string acc, ApplianceDbService applianceDbService, ApplianceConsumptionDbService applianceConsumptionDbService, OwnerPaymentDbService ownerPaymentDbService)
	{
		InitializeComponent();
        _acc = acc;
        _applianceDbService = applianceDbService;
        _applianceConsumptionDbService = applianceConsumptionDbService;
        _ownerDbService = ownerPaymentDbService;
        ApiBaseUrl apiBaseUrl = new ApiBaseUrl { ApiUrl = "https://localhost:7203" };
        _houseConsumptionDbService = new HouseConsumptionDbService(apiBaseUrl);
        this.Detail = new NavigationPage(new DashboardPage(acc, applianceConsumptionDbService));
    }

    private void BillsClicked(object sender, EventArgs e)
    {
       this.Detail = new NavigationPage( new Bills(_acc, _applianceConsumptionDbService, _ownerDbService));
        this.IsPresented = false;
    }

    private void HomeClicked(object sender, EventArgs e)
    {
        this.Detail = new NavigationPage (new DashboardPage(_acc, _applianceConsumptionDbService));
        this.IsPresented = false;
    }

    private async void LeakageClicked(object sender, EventArgs e)
    {
        Models.HouseConsumption currentHouseData = await GetHouseConsumption(_acc);
        List<ApplienceCosumption> rawHistory = await GetApplienceCosumptions();

        this.Detail = new NavigationPage(new LeakageView(_acc, currentHouseData, rawHistory));
        this.IsPresented = false;
    }

    private void AppliancesClicked(object sender, EventArgs e)
    {
        this.Detail = new NavigationPage(new MyAppliances(_acc, _applianceDbService, _applianceConsumptionDbService, _ownerDbService));
        this.IsPresented = false;
    }

    private async Task<Models.HouseConsumption> GetHouseConsumption(string acc)
    {
        var response = await _houseConsumptionDbService.GetHouseConsumption(acc);
        if (response.Data == null)
            return new Models.HouseConsumption();
        Models.HouseConsumption houseConsumption = new Models.HouseConsumption
        {
            House_Id = int.Parse(response.Data.Household_ID),
            Owner_Id = response.Data.Owner_AccountNumber,
            Water_Received = response.Data.Water_Received.Value,
            Total_WaterUsed = response.Data.Total_WaterUsed.Value,
            Leakage_Propability = response.Data.Leakage_Percent.Value,
            Month_OfConsumption = response.Data.Date.Value
        };
        return houseConsumption;
    }
    private async Task<List<Models.ApplienceCosumption>> GetApplienceCosumptions()
    {
        var dataFromDb = await _applianceConsumptionDbService.GetAllApplianceConsumptions();
        List<ApplienceCosumption> userConsumption = dataFromDb.Data.Select(x => new ApplienceCosumption
        {
            Appliance_Id = x.Id,
            Appliance_Name = x.Appliance_Name,
            Applience_Amount_Spent = x.Cost.Value,
            Applience_Water_Consumed = (x.Daily_Usage.Value + x.Night_Usage.Value),
            Status_Of_Spending = x.Daily_Status,
            Account_Number = x.Account,
            Spending_Date = x.Date.Value
        }).ToList();
        return userConsumption;
    }
}