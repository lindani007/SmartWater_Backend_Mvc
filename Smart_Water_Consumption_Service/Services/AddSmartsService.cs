using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Smart_Water_Consumption_Service.Models;
using Smart_Water_Consumption_Service.Services.DbServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smart_Water_Consumption_Service.Services
{
    public static class AddSmartsService
    {
        public static void AddSmartWaterServicesExtension(this IServiceCollection services, Action<ApiBaseUrl> configure)
        {
            services.Configure(configure);

            services.AddSingleton(provider =>
            {
                var apiBaseUrl = provider.GetRequiredService<IOptions<ApiBaseUrl>>().Value;
                return new LoginDbService(apiBaseUrl);
            });

            services.AddSingleton(provider =>
            {
                var apiBaseUrl = provider.GetRequiredService<IOptions<ApiBaseUrl>>().Value;
                return new ApplianceConsumptionDbService(apiBaseUrl);
            });

            services.AddSingleton(provider =>
            {
                var apiBaseUrl = provider.GetRequiredService<IOptions<ApiBaseUrl>>().Value;
                return new ApplianceDbService(apiBaseUrl);
            });

            services.AddSingleton(provider =>
            {
                var apiBaseUrl = provider.GetRequiredService<IOptions<ApiBaseUrl>>().Value;
                return new EmployeeDbService(apiBaseUrl);
            });

            services.AddSingleton(provider =>
            {
                var apiBaseUrl = provider.GetRequiredService<IOptions<ApiBaseUrl>>().Value;
                return new HouseConsumptionDbService(apiBaseUrl);
            });

            services.AddSingleton(provider =>
            {
                var apiBaseUrl = provider.GetRequiredService<IOptions<ApiBaseUrl>>().Value;
                return new OwnerPaymentDbService(apiBaseUrl);
            });

            services.AddSingleton(provider =>
            {
                var apiBaseUrl = provider.GetRequiredService<IOptions<ApiBaseUrl>>().Value;
                return new RegionConsumptionDbService(apiBaseUrl);
            });

            services.AddSingleton(provider =>
            {
                var apiBaseUrl = provider.GetRequiredService<IOptions<ApiBaseUrl>>().Value;
                return new RegionDbService(apiBaseUrl);
            });

            services.AddSingleton(provider =>
            {
                var apiBaseUrl = provider.GetRequiredService<IOptions<ApiBaseUrl>>().Value;
                return new Sub_RegionConsumptionDbService(apiBaseUrl);
            });

            services.AddSingleton(provider =>
            {
                var apiBaseUrl = provider.GetRequiredService<IOptions<ApiBaseUrl>>().Value;
                return new Sub_RegionDbService(apiBaseUrl);
            });
            services.AddSingleton(provider =>
            {
                var apiBaseUrl = provider.GetRequiredService<IOptions<ApiBaseUrl>>().Value;
                return new OwnerDbService(apiBaseUrl);
            });

            services.AddSingleton(provider =>
            {
                var apiBaseUrl = provider.GetRequiredService<IOptions<ApiBaseUrl>>().Value;
                return new HouseholdPredictionDbService(apiBaseUrl);
            });

            services.AddSingleton(provider =>
            {
                var apiBaseUrl = provider.GetRequiredService<IOptions<ApiBaseUrl>>().Value;
                return new RegionPredictionDbService(apiBaseUrl);
            });

            services.AddSingleton(provider =>
            {
                var apiBaseUrl = provider.GetRequiredService<IOptions<ApiBaseUrl>>().Value;
                return new Sub_RegionPredictionDbService(apiBaseUrl);
            });
        }
    }
}
