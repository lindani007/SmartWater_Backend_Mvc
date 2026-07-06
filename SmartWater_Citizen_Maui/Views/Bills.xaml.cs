using Smart_Water_Consumption_Service.Services.DbServices;
using SmartWater_Citizen_Maui.ViewModels;

namespace SmartWater_Citizen_Maui.Views;

public partial class Bills : ContentPage
{
	public Bills(string acc, ApplianceConsumptionDbService applianceConsumptionDbService, OwnerPaymentDbService ownerPaymentDbService)
	{
		InitializeComponent();
		BillsViewModel vm = new BillsViewModel(applianceConsumptionDbService, ownerPaymentDbService);
		BindingContext = vm;
		vm.LoadBillingData(acc);
	}
}