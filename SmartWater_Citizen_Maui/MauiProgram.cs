using Microsoft.Extensions.Logging;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microcharts.Maui;
using Smart_Water_Consumption_Service;
using Smart_Water_Consumption_Service.Services;
using Smart_Water_Consumption_Service.Models;
namespace SmartWater_Citizen_Maui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMicrocharts()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            builder.Services.AddSmartWaterServicesExtension(options =>
            {
                ApiBaseUrl apiBaseUrl = new ApiBaseUrl { ApiUrl = "https://localhost:7203" };
            });

#if DEBUG
            builder.Logging.AddDebug();
#endif


            return builder.Build();
        }
    }
}

