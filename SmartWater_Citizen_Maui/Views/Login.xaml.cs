
using Smart_Water_Consumption_Service.Services.DbServices;
using SmartWater_Citizen_Maui.ViewModels;

namespace SmartWater_Citizen_Maui.Views;

public partial class Login : ContentPage
{
    LoginDbService _dbService;
    OwnerDbService _ownerDbService;
    private readonly ApplianceDbService _applianceDbService;
    private readonly ApplianceConsumptionDbService _applianceConsumptionDbService;
    private readonly OwnerPaymentDbService _paymentService;
	public Login(LoginDbService loginDbService, OwnerDbService ownerDbService, 
        ApplianceDbService applianceDbService, ApplianceConsumptionDbService applianceConsumptionDbService, OwnerPaymentDbService ownerPaymentDbService)
	{
		InitializeComponent();
        LoginViewModel vm = new LoginViewModel(loginDbService, ownerDbService, applianceDbService, applianceConsumptionDbService, ownerPaymentDbService);
        _dbService = loginDbService;
        _ownerDbService = ownerDbService;
        _applianceDbService = applianceDbService;
        _applianceConsumptionDbService = applianceConsumptionDbService;
        _paymentService = ownerPaymentDbService;
        BindingContext = vm;
	}
    protected override bool OnBackButtonPressed()
    {
        return true;
    }

    private async void ForgotPassword_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new ForgotPassword(_dbService, _ownerDbService, _applianceDbService, _applianceConsumptionDbService, _paymentService));
    }

    private void ShowPasswordTapped(object sender, TappedEventArgs e)
    {
        if(paswordEntry.IsPassword)
            paswordEntry.IsPassword = false;
        else
            paswordEntry.IsPassword = true;

        if (pasword2Entry.IsPassword)
            pasword2Entry.IsPassword = false;
        else
            pasword2Entry.IsPassword = true;
    }
}