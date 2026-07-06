using SmartWater_Citizen_Maui.Views;

namespace SmartWater_Citizen_Maui.ViewModels;

public partial class MonthlyBills : ContentPage
{
	public MonthlyBills(BillingHistorySummary summary)
	{
		InitializeComponent();
		BindingContext = summary;
	}
}