using SmartWater_Citizen_Maui.ViewModels;

namespace SmartWater_Citizen_Maui.Views;

public partial class SettleBills : ContentPage
{
	public SettleBills()
	{
		InitializeComponent();
		SettleBillsViewModel viewModel = new SettleBillsViewModel();
		BindingContext = viewModel;
	}
}