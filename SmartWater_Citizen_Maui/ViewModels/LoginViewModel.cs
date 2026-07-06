using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Smart_Water_Consumption_Service.Services.DbServices;
using Smart_Water_Consumption_Service.Services.MailService;
using SmartWater_Citizen_Maui.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.ExceptionServices;
using System.Text;

namespace SmartWater_Citizen_Maui.ViewModels
{
    public partial class LoginViewModel: ObservableObject
    {
        [ObservableProperty]
        private string accountNumber;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string code;

        [ObservableProperty]
        private string confirmPassword;

        [ObservableProperty]
        private bool isChangePasswordCanBeVisble;

        [ObservableProperty]
        private bool isVerifyEmailCanBeVisble = true;

        [ObservableProperty]
        private bool isVerifyCodeCanBeVisble;

        [ObservableProperty]
        private bool isLoginCanBeVisble = true;

        [ObservableProperty]
        private bool isRegisterCanBeVisble;

        [ObservableProperty]
        private bool isIndicatorCanBeVisble;

        [ObservableProperty]
        private bool isVerifying;

        [ObservableProperty]
        private bool isLoginButtonCanBeVisble = true;

        [ObservableProperty]
        private bool isLabelCanBeVisble;

        private readonly LoginDbService _loginDbService;
        private readonly OwnerDbService _ownerDbService;
        private readonly ApplianceDbService _applianceDbService;
        ApplianceConsumptionDbService _applianceConsumptionDbService;
        OwnerPaymentDbService _ownerPaymentDbService;

        public LoginViewModel(LoginDbService loginDbService, OwnerDbService ownerDbService, ApplianceDbService applianceDbService, 
            ApplianceConsumptionDbService applianceConsumptionDbService, OwnerPaymentDbService ownerPaymentDbService)
        {
            _loginDbService = loginDbService;
            _ownerDbService = ownerDbService;
            _applianceDbService = applianceDbService;
            _applianceConsumptionDbService = applianceConsumptionDbService;
            _ownerPaymentDbService = ownerPaymentDbService;
        }

        [RelayCommand]
        private async Task LoginClicked()
        {
            IsVerifying = true;
            isLabelCanBeVisble = true;
            IsLoginButtonCanBeVisble = false;
            IsIndicatorCanBeVisble = true;
            string results = await VerifyLogin(AccountNumber, Password);
            IsVerifying = false;
            isLabelCanBeVisble = false;
            IsLoginButtonCanBeVisble = true;
            IsIndicatorCanBeVisble = false;
            if (results != "Login Sucesfuly")
            {
                await App.Current.MainPage.DisplayAlertAsync("Login Failed", results, "Retry");
                return;
            }
            await App.Current.MainPage.DisplayAlertAsync("Login Sucesful","Welcome to Smart Consumption","Okay");

            bool haveAppliances = await DoesUserHasAddedAnyAppliances(AccountNumber);
            if (haveAppliances)
                App.Current.MainPage = new Home(AccountNumber, _applianceDbService, _applianceConsumptionDbService, _ownerPaymentDbService);
            else
                await App.Current.MainPage.Navigation.PushAsync(new AddAplienceView(AccountNumber, _applianceDbService,_applianceConsumptionDbService, _ownerPaymentDbService));
            AccountNumber = string.Empty;
            Password = string.Empty;
        }

        [RelayCommand]
        private async Task RegisterClicked()
        {
            string results = await RegisterUser(AccountNumber,Email, Password);
            if (results != "User registered sucsfuly.")
            {
                await App.Current.MainPage.DisplayAlertAsync("Registration Failed", results, "Retry");
                return;
            }

            await App.Current.MainPage.DisplayAlertAsync("Register Sucesful", "Welcome to Smart Consumption", "Okay");
            IsLoginCanBeVisble = false;
            IsRegisterCanBeVisble = true;
            AccountNumber = string.Empty;
            Email = string.Empty;
            Password = string.Empty;
        }

        [RelayCommand]
        private async Task GoToLoginClicked()
        {
            IsRegisterCanBeVisble = false;
            IsLoginCanBeVisble = true;
        }

        [RelayCommand]
        private async Task GoToRegisterClicked()
        {
            IsLoginCanBeVisble = false;
            IsRegisterCanBeVisble = true;
        }


        [RelayCommand]
        private async Task SendClicked()
        {
            string results = await VerifyEmail(Email, AccountNumber);
            if (results != "Email is verfied. You can proced with changing you password")
            {
                await App.Current.MainPage.DisplayAlertAsync("Verification Failed", results, "Retry");
                return;
            }

            await App.Current.MainPage.DisplayAlertAsync("Verification Code Sent Successfully", $"We have sent verification code to {Email}. Please check your spam if it does not appear.", "Okay");
            IsVerifyEmailCanBeVisble = false;
            IsVerifyCodeCanBeVisble = true;
        }

        [RelayCommand]
        private async Task VerfyClicked()
        {
            string results = VerifyCode(Code);
            if (results != "Code verified succesful.")
            {
                await App.Current.MainPage.DisplayAlertAsync("Verification Failed", results, "Retry");
                return;
            }

            await App.Current.MainPage.DisplayAlertAsync("Verification Sucesful", "Code verified succesful.", "Okay");
            IsVerifyCodeCanBeVisble = false;
            IsChangePasswordCanBeVisble = true;
        }

        [RelayCommand]
        private async Task ChangeClicked()
        {
            string results = await ChangePassword(Password, ConfirmPassword, AccountNumber);
            if (results != "Password changed sucsfuly.")
            {
                await App.Current.MainPage.DisplayAlertAsync("Changing Failed", results, "Retry");
                return;
            }

            await App.Current.MainPage.DisplayAlertAsync("Password Changed Successfully", "You can now login using your new password.", "Okay");
            Password = string.Empty;
            ConfirmPassword = string.Empty;
            Email = string.Empty;
            await App.Current.MainPage.Navigation.PushAsync(new Login(_loginDbService, _ownerDbService, _applianceDbService, _applianceConsumptionDbService, _ownerPaymentDbService));
        }
        private async Task<string> VerifyLogin(string accountNum, string password)
        {
            if (string.IsNullOrEmpty(accountNum) && string.IsNullOrEmpty(password))
                return "Account number and password are required";
            else if (string.IsNullOrEmpty(accountNum))
                return "Account number is required";
            else if (string.IsNullOrEmpty(password))
                return "Password is required";
            else
            {
                Models.Login storedUser = await GetUserFromDb(accountNum);
                if (string.IsNullOrEmpty(storedUser.Account_Number) || string.IsNullOrWhiteSpace(storedUser.Password))
                    return $"User with {accountNum} does not exists";
                bool isPasswordCorrect = BCrypt.Net.BCrypt.EnhancedVerify(password, storedUser.Password);

                if (storedUser.Account_Number == accountNum && isPasswordCorrect)
                    return "Login Sucesfuly";
                else if (storedUser.Account_Number == accountNum && !isPasswordCorrect)
                    return "Incorrect password";
                else
                    return $"User with {AccountNumber} does not exist";
            }
        }

        private async Task<string> RegisterUser(string accountNum, string email, string password)
        {
            if (string.IsNullOrEmpty(accountNum) && string.IsNullOrEmpty(password) && string.IsNullOrEmpty(email))
                return "All fields are required";
            else if (string.IsNullOrEmpty(accountNum))
                return "Account number is required";
            if (accountNum.Length < 10)
                return "Account Number must be 10 charecters";
            else if (string.IsNullOrEmpty(email))
                return "Email is required";
            else if (!new EmailAddressAttribute().IsValid(email))
                return "Please enter a correct email address format";
            else if (string.IsNullOrEmpty(password))
                return "Password is required";
            else
            {
                Models.Login login = await GetUserFromDb(accountNum);

                if(string.IsNullOrEmpty( login.Account_Number) && string.IsNullOrEmpty(login.Password) && !string.IsNullOrEmpty(email))
                {
                    var users = await GetAllUsers();
                    bool doesemailExist = users.Any(x => x.Email ==  email);
                    if (doesemailExist)
                        return $"{email} already exists in our records";
                    string ownerName = await CheckIfUserExistAtMunicipalList(accountNum);
                    if (string.IsNullOrEmpty(ownerName))
                        return $"Your account number {accountNum} does not exists in the municipal list";
                    string hashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(password, 12);
                    Smart_Water_Consumption_Service.Models.Login userToStore = new Smart_Water_Consumption_Service.Models.Login
                    {
                        Account_Number = accountNum,
                        Password = hashedPassword,
                        Email_Address = email,
                        Role = "Citezen",
                        Staff_Id = string.Empty,
                        FullNames = ownerName
                    };
                    var response = await _loginDbService.AddUser(userToStore);
                    if (response.IsSuccess)
                        return $"User registered sucsfuly.";
                    else
                        return response.Message;
                }
                return $"User {accountNum} already registered. Please Login or click Fortgot Password if you forgot it.";
            }
        }
        private async Task<Models.Login> GetUserFromDb(string account)
        {
            var response = await _loginDbService.GetCitezen(account);
            if(response.IsSuccess && response.Data != null)
            {
                Models.Login login = new Models.Login
                {
                    Account_Number = response.Data.Account_Number,
                    Email = response.Data.Email_Address,
                    Password = response.Data.Password,
                    FullNames= response.Data.FullNames,
                    Id = response.Data.Id,
                    Role = response.Data.Role,
                };
                return login;
            }
            return new Models.Login();
        }
        public async Task<string> CheckIfUserExistAtMunicipalList(string acc)
        {
            var response = await _ownerDbService.GetOwner(acc);
            if (response.IsSuccess && response.Data != null)
                return response.Data.Owner_Name;
            else
                return string.Empty;
        }
        private async Task<string> VerifyEmail(string email, string acc)
        {
            if (string.IsNullOrEmpty(email))
                return "Email is required";
            else if (!new EmailAddressAttribute().IsValid(email))
                return "Please enter a correct email address format";
            else
            {
                Models.Login login = await GetUserFromDb(acc);
                if (login == null)
                    return "Unable to get your details, please re-enter your account number";

                string name = login.FullNames ?? string.Empty;
                if (login.Email == email)
                {
                    string code = CreateCode();
                    Preferences.Default.Set("UserCode", code);
                    Preferences.Default.Set("CodeTimeStamp", DateTime.Now.ToString("O"));
                    await EmailSender.SendCode(email, name, code);
                    return "Email is verfied. You can proced with changing you password";
                }
                else
                    return $"We could not found any record with this kind of email {email} or account number {acc}";
            }
        }
        private string VerifyCode(string code)
        {
            if (string.IsNullOrEmpty(code))
                return "Code is required";
            else
            {
                string savedCode = Preferences.Default.Get("UserCode", string.Empty);
                string timeStampString = Preferences.Default.Get("CodeTimeStamp", string.Empty);

                if (string.IsNullOrEmpty(savedCode) || string.IsNullOrEmpty(timeStampString))
                {
                    return "Please request a new code we had trouble with storing your code.";
                }

                DateTime creationTime = DateTime.Parse(timeStampString);

                TimeSpan timePassed = DateTime.Now - creationTime;

                if (timePassed.TotalMinutes > 3)
                {
                    Preferences.Default.Remove("UserCode");
                    Preferences.Default.Remove("CodeTimeStamp");
                    return "Your code has expired. Please request a new one and Note*: It will expire after 3 minutes.";
                }

                if (code == savedCode)
                {
                    Preferences.Default.Remove("UserCode");
                    Preferences.Default.Remove("CodeTimeStamp");
                    return "Code verified succesful.";
                }

                return "Incorrect code please enter a correct code";
            }
        }
        private async Task<string> ChangePassword(string password, string conpassword, string acc)
        {
            if (string.IsNullOrEmpty(password) && string.IsNullOrEmpty(conpassword))
                return "All fields are required";
            else if (string.IsNullOrEmpty(password))
                return "Password is required";
            if (password.Length < 8)
                return "Password must be 8 charecters in minimum length";
            else if (string.IsNullOrEmpty(conpassword))
                return "Confirm Password is required";
            else if (password != conpassword)
                return "Passwords does not match";
            else
            {
                var user = await GetUserFromDb(acc);
                string hashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(password, 12);
                Smart_Water_Consumption_Service.Models.Login userToUpdate = new Smart_Water_Consumption_Service.Models.Login
                {
                    Id = user.Id,
                    Account_Number = user.Account_Number,
                    Email_Address = user.Email,
                    Password = hashedPassword,
                    FullNames = user.FullNames,
                    Role = user.Role
                };

                var response = await _loginDbService.UpdatePassword(userToUpdate);
                if (response.IsSuccess)
                    return $"Password changed sucsfuly.";
                else
                    return response.Message;
            }
        }
        private string CreateCode()
        {
            Random random = new Random();
            int code = random.Next(1000, 10000);
            return code.ToString();
        }
        private async Task<List<Models.Login>> GetAllUsers()
        {
            var response = await _loginDbService.ReturnUsers();
            List<Models.Login> users = response.Select(x => new Models.Login
            {
                Id = x.Id,
                Account_Number = x.Account_Number,
                FullNames = x.FullNames,
                Email = x.Email_Address,
                Password = x.Password,
            }).ToList();
            return users;
        }
        private async Task<bool> DoesUserHasAddedAnyAppliances(string acc)
        {
            var appliances = await _applianceDbService.GetAllAppliance();
            return appliances.Data.Any(x => x.Owner_AccountNumber == acc);
        }
    }
}
