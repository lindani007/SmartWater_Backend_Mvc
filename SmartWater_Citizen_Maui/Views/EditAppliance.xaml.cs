using SmartWater_Citizen_Maui.ViewModels;

namespace SmartWater_Citizen_Maui.Views;

public partial class EditAppliance : ContentPage
{
	public EditAppliance(ApplienceViewModel applienceViewModel)
	{
		InitializeComponent();
		BindingContext = applienceViewModel;
	}
}