
using Smart_Water_Consumption_Service.Services.DbServices;
using SmartWater_Citizen_Maui.ViewModels;

namespace SmartWater_Citizen_Maui.Views;

public partial class MyAppliances : ContentPage
{
	public MyAppliances(string acc, ApplianceDbService applianceDbService, ApplianceConsumptionDbService applianceConsumptionDbService, OwnerPaymentDbService ownerPaymentDbService)
	{
		InitializeComponent();
		ApplienceViewModel applienceViewModel = new ApplienceViewModel(acc, applianceDbService, applianceConsumptionDbService, ownerPaymentDbService);
		BindingContext = applienceViewModel;
	}
}