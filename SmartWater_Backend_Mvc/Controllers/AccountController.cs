using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Mvc;
using Smart_Water_Consumption_Service.Models;
using Smart_Water_Consumption_Service.Services.DbServices;
using Smart_Water_Consumption_Service.Services.MailService;
using SmartWater_Backend_Mvc.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.Xml;

namespace SmartWater_Backend_Mvc.Controllers
{
    public class AccountController : Controller
    {
        LoginDbService _loginDbService;
        public AccountController(LoginDbService loginDbService1)
        {
            _loginDbService = loginDbService1;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(Models.Login login)
        {
            if(ModelState.IsValid)
            {
                HttpContext.Session.SetString("stuffId", login.Stuff_Id);
                string result = await VerifyLogin(login.Stuff_Id, login.Password);
                if(result == "User Login succesfuly. finance")
                    return RedirectToAction("FinanceView", "Finance");
                else if (result == "User Login succesfuly. engineer")
                    return RedirectToAction("Regions", "Engineer");
                else if (result == "User Login succesfuly. admin")
                    return RedirectToAction("AdminDashboard", "Admin");
                else if (result == "User Login succesfuly. clerk")
                    return RedirectToAction("Citezens", "Clerk");

                HttpContext.Session.Remove("stuffId");
                ViewData["Message"] = result;
                return View();
            }
            ViewData["Message"] = VerifyLogin(login.Stuff_Id, login.Password);
            return View(login);
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(Models.Login login)
        {
            string results = await VerifyEmail(login.Email ?? string.Empty, login.Stuff_Id ?? string.Empty);
            if(results == "Email is verfied. You can proced with changing you password")
            {
                HttpContext.Session.SetString("IdThatChangingPassword", login.Stuff_Id ?? string.Empty);
                return View("VerfiyCode");
            }
            ViewData["Message"] = results;
            return View(login);
        }
        [HttpGet]
        public IActionResult VerfiyCode()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VerfiyCode(Models.Login login)
        {
            string results = await VerifyCode(login.Role);
            if (results == "Code verified succesful.")
                return View("ChangePassword");

            ViewData["Message"] = results;
            return View(login);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePassword changePassword)
        {
            string stuffId = HttpContext.Session.GetString("IdThatChangingPassword") ?? string.Empty;
            string results = await ChangePassword(changePassword.Password ?? string.Empty,changePassword.ConfirmPassword ?? string.Empty, stuffId);
            if (results == "Password changed sucsfuly.")
                return View("Login");

            ViewData["Message"] = results;
            return View(changePassword);
        }

        public async Task<string> VerifyLogin(string staffId, string password)
        {
            if (string.IsNullOrEmpty(staffId) && string.IsNullOrEmpty(password))
                return "All fields are required";
            else if (string.IsNullOrEmpty(staffId))
                return "stuff Id is required";
            if (staffId.Length < 6)
                return "Stuff Id must be 6 charecters";
            else if (string.IsNullOrEmpty(password))
                return "Password is required";
            else
            {
                Models.Login user = await GetUserFromDb(staffId);
                if (string.IsNullOrEmpty(user.Stuff_Id) || string.IsNullOrEmpty(user.Password))
                    return "User does not exists";
                string finance = "Finance";
                string engineer = "Engineer";
                string admin = "Admin";
                string clerk = "Clerk";
                bool isPasswordCorrect = BCrypt.Net.BCrypt.EnhancedVerify(password, user.Password);

                if(user.Role == finance)
                {
                    if (user.Stuff_Id == staffId && isPasswordCorrect)
                        return $"User Login succesfuly. finance";
                    else if (user.Stuff_Id == staffId && !isPasswordCorrect)
                        return "Password is incorrect";
                }
                else if(user.Role == engineer)
                {
                    if (user.Stuff_Id == staffId && isPasswordCorrect)
                        return $"User Login succesfuly. engineer";
                    else if (staffId == engineer && !isPasswordCorrect)
                        return "Password is incorrect";
                }
                else if(user.Role == admin)
                {
                    if (user.Stuff_Id == staffId && isPasswordCorrect)
                        return $"User Login succesfuly. admin";
                    else if (user.Stuff_Id == staffId && !isPasswordCorrect)
                        return "Password is incorrect";
                }
                else if (user.Role == clerk)
                {
                    if (user.Stuff_Id == staffId && isPasswordCorrect)
                        return $"User Login succesfuly. clerk";
                    else if (user.Stuff_Id == staffId && !isPasswordCorrect)
                        return "Password is incorrect";
                }
                return $"User with {staffId} does not exist";
            }
        }
        private async Task<Models.Login> GetUserFromDb(string staffId)
        {
            var response = await _loginDbService.GetEmployee(staffId);
            if (response.IsSuccess && response.Data != null)
            {
                Models.Login login = new Models.Login
                {
                    Stuff_Id = response.Data.Staff_Id,
                    Email = response.Data.Email_Address,
                    Password = response.Data.Password,
                    Fullnames = response.Data.FullNames,
                    Id = response.Data.Id,
                    Role = response.Data.Role,
                };
                return login;
            }
            return new Models.Login();
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

                string name = login.Fullnames ?? string.Empty;
                if (login.Email == email)
                {
                    string code = CreateCode();
                    HttpContext.Session.SetString("UserCode", code);
                    HttpContext.Session.SetString("CodeTimeStamp", DateTime.Now.ToString("O"));
                    await EmailSender.SendCode(email, name, code);
                    return "Email is verfied. You can proced with changing you password";
                }
                else
                    return $"We could not found any record with this kind of email {email} or account number {acc}";
            }
        }
        private async Task<string> VerifyCode(string code)
        {
            if (string.IsNullOrEmpty(code))
                return "Code is required";
            else
            {
                string savedCode = HttpContext.Session.GetString("UserCode") ?? string.Empty;
                string timeStampString = HttpContext.Session.GetString("CodeTimeStamp") ?? string.Empty;

                if (string.IsNullOrEmpty(savedCode) || string.IsNullOrEmpty(timeStampString))
                {
                    return "Please request a new code we had trouble with storing your code.";
                }

                DateTime creationTime = DateTime.Parse(timeStampString);

                TimeSpan timePassed = DateTime.Now - creationTime;

                if (timePassed.TotalMinutes > 3)
                {
                    HttpContext.Session.Remove("UserCode");
                    HttpContext.Session.Remove("CodeTimeStamp");
                    return "Your code has expired. Please request a new one and Note*: It will expire after 3 minutes.";
                }

                if (code == savedCode)
                {
                    HttpContext.Session.Remove("UserCode");
                    HttpContext.Session.Remove("CodeTimeStamp");
                    return "Code verified succesful.";
                }

                return "Incorrect code please enter a correct code";
            }
        }
        private async Task<string> ChangePassword(string password, string conpassword, string STUFFiD)
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
                var user = await GetUserFromDb(STUFFiD);
                string hashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(password, 12);
                Smart_Water_Consumption_Service.Models.Login userToUpdate = new Smart_Water_Consumption_Service.Models.Login
                {
                    Id = user.Id,
                    Staff_Id = user.Stuff_Id,
                    Email_Address = user.Email,
                    Password = hashedPassword,
                    FullNames = user.Fullnames,
                    Role = user.Role ?? "Unknown",
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
    }
}
