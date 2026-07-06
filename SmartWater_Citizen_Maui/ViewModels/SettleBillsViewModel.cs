using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SmartWater_Citizen_Maui.ViewModels
{
    public partial class SettleBillsViewModel: ObservableObject
    {
        [ObservableProperty]
        private double amount_ToPay;

        [ObservableProperty]
        private string card_Number;
        [ObservableProperty]
        private string expiring_Date;
        [ObservableProperty]
        private string cSV;

        [RelayCommand]
        private async void PayClicked()
        {
            string results = VerifyPayment(Amount_ToPay, Card_Number, Expiring_Date, CSV);
            if(results == "Success")
            {
                await App.Current.MainPage.DisplayAlertAsync("Payment", $"Payment of R{Amount_ToPay} has been made succesfuly.", "Done");
                if (Application.Current?.MainPage is FlyoutPage flyoutPage && flyoutPage.Detail is NavigationPage navPage)
                {
                    // This looks inside the FlyoutPage, grabs the actual active Navigation stack, and safely pushes to it!
                    await navPage.PopAsync();
                }
                return;
            }
            await App.Current.MainPage.DisplayAlertAsync("Payment failed", results, "Retry");
        }

        public string VerifyPayment(double amt, string card_Num, string expiring_Date, string csv)
        {
            if (amt <= 0 || string.IsNullOrWhiteSpace(card_Num) || string.IsNullOrWhiteSpace(expiring_Date) || string.IsNullOrWhiteSpace(csv))
                return "All fields are required and amount must be greater than zero";

            else if (card_Num.Replace(" ", "").Replace("-", "").Length < 15 || card_Num.Replace(" ", "").Replace("-", "").Length > 16)
                return "Card number must be 15 or 16 digits long";

            else if (!long.TryParse(card_Num.Replace(" ", "").Replace("-", ""), out _))
                return "Card number must contain numeric values only";

            else if (!expiring_Date.Contains("/"))
                return "Expiry date format must contain a slash '/'";

            else if (expiring_Date.Trim().Length != 5)
                return "Expiry date format must be exactly 5 characters (e.g., 06/28)";

            else if (!int.TryParse(expiring_Date.Split('/')[0], out int month) || !int.TryParse(expiring_Date.Split('/')[1], out int year))
                return "Invalid expiry date characters";

            else if (month < 1 || month > 12)
                return "Expiry month must be between 01 and 12";

            else if (new DateTime((year < 100 ? 2000 + year : year), month, 1) < new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1))
                return "The provided card has expired";

            else if (csv.Trim().Length < 3 || csv.Trim().Length > 4 || !int.TryParse(csv.Trim(), out _))
                return "CSV/CVV security code must be 3 or 4 digits";

            else
                return "Success";
        }
    }
}
