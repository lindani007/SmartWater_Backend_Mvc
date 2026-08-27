using Microsoft.AspNetCore.Mvc;
using Smart_Water_Consumption_Service.Services.DbServices;
using Smart_Water_Consumption_Service.Services.SimulationService;
using SmartWater_Backend_Mvc.Models;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SmartWater_Backend_Mvc.Controllers
{
    public class AdminController : Controller
    {
        EmployeeDbService _employeeDbService;
        private readonly LoginDbService _loginDbService;
        private readonly OwnerDbService _ownerDbService;
        private readonly RegionConsumptionDbService _regionConsumptionDbService;
        public AdminController(EmployeeDbService employeeDbService, LoginDbService loginDbService, OwnerDbService ownerDbService, RegionConsumptionDbService regionConsumptionDbService) 
        {
            _employeeDbService = employeeDbService;
            _loginDbService = loginDbService;
            _ownerDbService = ownerDbService;
            _regionConsumptionDbService = regionConsumptionDbService;
        }
       
        public async Task<IActionResult> AdminDashboard()
        {
            try
            {
                WaterSimulationEngine _simulationEngine = new WaterSimulationEngine();
                _ = Task.Run(async () =>
                {
                    await _simulationEngine.RunSimulation();
                });
                return View(await GetRegionConsumptions());
            }
            catch (Exception ex)
            {
                ViewData["Message"] = ex.Message;
                return View(await GetRegionConsumptions());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Owners()
        {
            try
            {
                return View(await GetCitezens());

            }
            catch (Exception ex)
            {
                ViewData["Message"] = ex.Message;
                return View(await GetCitezens());
            }
        }

        [HttpGet]
        public async Task<IActionResult> DownloadOwnersReport()
        {
            try
            {
                var data = await GetCitezens();
                var csvBuilder = new StringBuilder();

                csvBuilder.AppendLine("Owner ID,Name,Account Number,Email,Phone,Sub-Region,Outstanding Balance,Status");

                foreach (var owner in data)
                {
                    string status = owner.IsPaid ? "Paid" : "Unpaid";
                    csvBuilder.AppendLine($"{owner.OwnerId_Number},{owner.OwnerName},{owner.Account_Number},{owner.Email},{owner.Phone},{owner.SubRegionName},R{owner.OutstandingBalance.ToString("F2")},{status}");
                }

                byte[] buffer = Encoding.UTF8.GetBytes(csvBuilder.ToString());
                return File(buffer, "text/csv", "SmartWater_Owners_Payment_Report.csv");
            }
            catch (Exception ex)
            {
                ViewData["Message"] = ex.Message;
                return View(await GetCitezens());
            }
        }

        [HttpGet]
        public  async Task<IActionResult> Employees()
        {
            try
            {
                return View(await GetAllEmployees());

            }
            catch (Exception ex)
            {
                ViewData["Message"] = ex.Message;
                return View(await GetAllEmployees());
            }
        }

        [HttpGet]
        public  IActionResult AddEmployee()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddEmployee(Employee employee)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    List<Employee> employees = await GetAllEmployees();
                    employee.Employer_Number = CreateEmployeeNumber(employees.Count);
                    var employeeToAdd = CreateDbEmployeeModel(employee);
                    var response = await _employeeDbService.AddEmployee(employeeToAdd);
                    if (response.IsSuccess)
                    {
                        employees = await GetAllEmployees();
                        string createEmployeeLoginAccount = await RegisterEmployee(employee);
                        if (createEmployeeLoginAccount == "User registered sucsfuly.")
                            return View("Employees", employees);

                        ViewData["Message"] = createEmployeeLoginAccount + "\n Unable to create a new employee account right now, please rty to add him/her manualy on the database. Employe details has been add succesful.";
                        return View();
                    }
                    ViewData["Message"] = response.Message;
                    return View(employee);
                }
                return View(employee);
            }
            catch (Exception ex)
            {
                ViewData["Message"] = ex.Message;
                return View(employee);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditEmployee(string id)
        {
            try
            {
                List<Employee> employees = await GetAllEmployees();
                var employee = employees.FirstOrDefault(x => x.Employer_Number == id);
                if(employee == null)
                {
                    ViewData["Message"] = "Employee not found.";
                    return View("Employees", employees);
                }
                return View(employee);
            }
            catch (Exception ex)
            {
                ViewData["Message"] = ex.Message;
                return View("Employees", await GetAllEmployees());
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditEmployee(Employee employee)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var employeeDb = new Smart_Water_Consumption_Service.Models.Employee
                    {
                        EmployeeIdNumber = employee.Employer_IdNumber ?? string.Empty,
                        Employee_Number = employee.Employer_Number ?? string.Empty,
                        EmployeeAddress = employee.Employee_Address ?? string.Empty,
                        EmployeeEmail = employee.Employer_Email ?? string.Empty,
                        EmployeePhone = employee.Employer_PhoneNumber ?? string.Empty,
                        EmployeeFullNames = employee.Employee_FullNames ?? string.Empty,
                        EmployeeRole = employee.Employer_Role ?? string.Empty
                    };
                    var response = await _employeeDbService.EditEmployee(employeeDb);
                    if (response.IsSuccess)
                    {
                        List<Employee> Newemployees = await GetAllEmployees();
                        return View("Employees", Newemployees);
                    }
                    ViewData["Message"] = response.Message;
                    return View(employee);
                }
                return View(employee);
            }
            catch (Exception ex)
            {
                ViewData["Message"] = ex.Message;
                return View(employee);
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteEmployee(string id)
        {
            try
            {
                var response = await _employeeDbService.DeleteEmployee(id);
                if (response.IsSuccess)
                {
                    List<Employee> NEWemployees = await GetAllEmployees();
                    return View("Employees", NEWemployees);
                }
                List<Employee> employees = await GetAllEmployees();
                return View("Employees", employees);
            }
            catch(Exception ex)
            {
                ViewData["Message"] = ex.Message;
                return View("Employees", await GetAllEmployees());
            }
        } 

        private  async Task<List<Models.Employee>> GetAllEmployees()
        {
            var employees = await _employeeDbService.GetAllEmployees();
            List<Models.Employee> employeesFromDb = employees.Data.Select(x => new Models.Employee
            {
                Employer_IdNumber = x.EmployeeIdNumber,
                Employee_FullNames = x.EmployeeFullNames,
                Employee_Address = x.EmployeeAddress,
                Employer_PhoneNumber = x.EmployeePhone,
                Employer_Email = x.EmployeeEmail,
                Employer_Number = x.Employee_Number,
                Employer_Role = x.EmployeeRole,
            }).ToList();
            return employeesFromDb;
        }
        private async Task<Employee> GetEmploye(string  employerId)
        {
            var response = await _employeeDbService.GetEmployee(employerId);
            if(response.IsSuccess)
            {
                var employee = new Models.Employee
                {
                    Employer_IdNumber = response.Data.EmployeeIdNumber,
                    Employee_FullNames = response.Data.EmployeeFullNames,
                    Employee_Address = response.Data.EmployeeAddress,
                    Employer_PhoneNumber = response.Data.EmployeePhone,
                    Employer_Email = response.Data.EmployeeEmail,
                    Employer_Number = response.Data.Employee_Number,
                    Employer_Role = response.Data.EmployeeRole,
                };
                return employee;
            }
            return new Employee();
        }
        private string CreateEmployeeNumber(int employeesNumber)
        {
            employeesNumber += 1;
            string employeeNumber;
                return employeeNumber = $"emp{employeesNumber.ToString("D3")}";
        }
        private Smart_Water_Consumption_Service.Models.Employee CreateDbEmployeeModel(Models.Employee employee)
        {
            var employeeDb = new Smart_Water_Consumption_Service.Models.Employee
            {
                EmployeeIdNumber = employee.Employer_IdNumber ?? string.Empty,
                Employee_Number = employee.Employer_Number ?? string.Empty,
                EmployeeAddress = employee.Employee_Address ?? string.Empty,
                EmployeeEmail = employee.Employer_Email ?? string.Empty,
                EmployeePhone = employee.Employer_PhoneNumber ?? string.Empty,
                EmployeeFullNames = employee.Employee_FullNames ?? string.Empty,
                EmployeeRole = employee.Employer_Role ?? string.Empty
            };
            return employeeDb;
        }
        private async Task<string> RegisterEmployee(Models.Employee employee)
        {
                    string hashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword("password01", 12);
                    Smart_Water_Consumption_Service.Models.Login userToStore = new Smart_Water_Consumption_Service.Models.Login
                    {
                        Account_Number = string.Empty,
                        Password = hashedPassword,
                        Email_Address = employee.Employer_Email,
                        Role = employee.Employer_Role ?? "UNKWNON",
                        Staff_Id = employee.Employer_Number,
                        FullNames = employee.Employee_FullNames
                    };
                    var response = await _loginDbService.AddUser(userToStore);
                    if (response.IsSuccess)
                        return "User registered sucsfuly.";
                    else
                        return response.Message;
        }

        private async Task<List<Models.Owner>> GetCitezens()
        {
            var response = await _ownerDbService.GetAllOwners();
            var citezens = response.Data.Select(x => new Models.Owner
            {
                Meter_Number = x.Owner_MeterNumber ?? string.Empty,
                OwnerName = x.Owner_Name ?? string.Empty,
                Account_Number = x.Owner_AccountNumber ?? string.Empty,
                Email = x.Owner_Email ?? string.Empty,
                Phone = x.Owner_PhoneNumber ?? string.Empty,
                SubRegionName = x.Sub_RegionName ?? string.Empty,
                OwnerId_Number = x.Owner_IdNumber ?? string.Empty,
                HouseId = int.Parse(x.Household_ID ?? "0"),
                PropertyValue = x.PropertyValue,
                OutstandingBalance = 0.00,
                
            }).ToList();

            return citezens;
        }
        private async Task<List<RegionConsumption>> GetRegionConsumptions()
        {
            var response = await _regionConsumptionDbService.GetAllRegionsConsumptions();
            var regions = response.Data.Select(x => new RegionConsumption
            {
                Id = x.Id,
                Region_Id = x.Region_Id,
                Region_Name = x.Region_Name,
                Water_Sent = x.Water_Sent.Value,
                Water_Used = x.Water_Payed.Value,
                Water_Thats_OutStanding = x.OutstandingWater.Value,
                Expected_Cost = x.Extepected_Amount.Value,
                Actual_Cost = x.Actual_Amount.Value,
                FreeWater = x.FreeWater,
                Consumption_Month = x.Date.Value,
                OutStanding_Cost = x.OutStanding_Amount.Value,
                Leakage_Propability = x.Leakage_Percent.Value,
            }).ToList();
            return regions;
        }
    }
}
