using Smart_Water_Consumption_Service;
using Smart_Water_Consumption_Service.Models;
using Smart_Water_Consumption_Service.Services;
using Smart_Water_Consumption_Service.Services.DbServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSmartWaterServicesExtension(options =>
{
    options.ApiUrl = "https://localhost:7203";
});
ApiBaseUrl apiBaseUrl = new ApiBaseUrl { ApiUrl = "https://localhost:7203" };
RegionDbService regionDbService = new RegionDbService(apiBaseUrl);
RegionConsumptionDbService regionConsumptionDbService = new RegionConsumptionDbService(apiBaseUrl);
Sub_RegionDbService sub_RegionDbService = new Sub_RegionDbService(apiBaseUrl);
Sub_RegionConsumptionDbService sub_RegionConsumptionDbService = new Sub_RegionConsumptionDbService(apiBaseUrl);
OwnerDbService ownerDbService = new OwnerDbService(apiBaseUrl);
OwnerPaymentDbService ownerPayment = new OwnerPaymentDbService(apiBaseUrl);
ApplianceConsumptionDbService applianceConsumptionDbService = new ApplianceConsumptionDbService(apiBaseUrl);
HouseConsumptionDbService houseConsumptionDbService = new HouseConsumptionDbService(apiBaseUrl);
EmployeeDbService employeeDbService = new EmployeeDbService(apiBaseUrl);
LoginDbService loginDbService = new LoginDbService(apiBaseUrl);
ApplianceDbService applianceDbService = new ApplianceDbService(apiBaseUrl);

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSession();
app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}")
    .WithStaticAssets();


app.Run();
