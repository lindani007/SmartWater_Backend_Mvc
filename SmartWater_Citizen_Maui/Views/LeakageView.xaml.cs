using SmartWater_Citizen_Maui.Models;
using SmartWater_Citizen_Maui.ViewModels;

namespace SmartWater_Citizen_Maui.Views;

public partial class LeakageView : ContentPage
{
    private readonly LeakageViewModel _viewModel;
    public LeakageView(string account, HouseConsumption currentHouseData, List<ApplienceCosumption> rawHistory)
	{
		InitializeComponent();
        _viewModel = new LeakageViewModel();
        this.BindingContext = _viewModel;


        _viewModel.LoadLeakageData(account, currentHouseData, rawHistory);
    }
}