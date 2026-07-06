using Smart_Water_Consumption_Service.Services.DbServices;
using SmartWater_Citizen_Maui.ViewModels;

namespace SmartWater_Citizen_Maui.Views;

public partial class ForgotPassword : ContentPage
{
	public ForgotPassword(LoginDbService loginDbService, OwnerDbService ownerDbService, ApplianceDbService applianceDbService, 
		ApplianceConsumptionDbService applianceConsumptionDbService, OwnerPaymentDbService ownerPaymentDbService)
	{
		InitializeComponent();
		LoginViewModel VM = new LoginViewModel(loginDbService, ownerDbService, applianceDbService, applianceConsumptionDbService, ownerPaymentDbService);
		BindingContext = VM;
	}
}