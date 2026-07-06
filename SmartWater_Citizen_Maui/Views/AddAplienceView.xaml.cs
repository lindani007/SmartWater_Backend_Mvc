using Smart_Water_Consumption_Service.Services.DbServices;
using SmartWater_Citizen_Maui.ViewModels;

namespace SmartWater_Citizen_Maui.Views;

public partial class AddAplienceView : ContentPage
{
	public AddAplienceView(string account, ApplianceDbService applianceDbService, 
		ApplianceConsumptionDbService applianceConsumptionDbService, OwnerPaymentDbService ownerPaymentDbService)
	{
		InitializeComponent();
		ApplienceViewModel vm = new ApplienceViewModel(account, applianceDbService, applianceConsumptionDbService, ownerPaymentDbService);
		BindingContext = vm;
	}
}