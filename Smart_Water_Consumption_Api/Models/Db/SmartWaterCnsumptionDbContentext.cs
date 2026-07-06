using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Smart_Water_Consumption_Api.Models.Db
{
    public class SmartWaterCnsumptionDbContentext: DbContext
    {
        public DbSet<Smart_Water_Consumption_Api.Models.SubRegionPrediction> SubRegionPrediction { get; set; } = default!;
        public DbSet<Smart_Water_Consumption_Api.Models.RegionPrediction> RegionPrediction { get; set; } = default!;
        public DbSet<Smart_Water_Consumption_Api.Models.HouseholdPrediction> HouseholdPrediction { get; set; } = default!;
        public SmartWaterCnsumptionDbContentext(DbContextOptions<SmartWaterCnsumptionDbContentext> dbContextOptions) : base(dbContextOptions) { }

        public DbSet<Owner> Owners { get; set; }
        public DbSet<HouseConsumption> HouseConsumptions { get; set; }
        public DbSet<OwnerPayment> OwnerPayments { get; set; }
        public DbSet<Appliance> Appliances { get; set; }
        public DbSet<ApplianceConsumption> ApplianceConsumptions { get; set; }
        public DbSet<Login> Logins { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<RegionConsumption> RegionConsumptions { get; set; }
        public DbSet<Sub_Region> Sub_Regions { get; set; }
        public DbSet<Sub_RegionConsumption> Sub_RegionConsumptions { get; set; }
        public DbSet<HouseholdPrediction> HouseholdPredictions { get; set; }
        public DbSet<RegionPrediction> RegionPredictions { get; set; }
        public DbSet<SubRegionPrediction> SubRegionPredictions { get; set; }
        public DbSet<AppliancePrediction> AppliancePredictions { get; set; }
        public DbSet<ModelTrainingPayload> ModelTrainingPayloads { get; set; }
    }

}
