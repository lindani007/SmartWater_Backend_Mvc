using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Smart_Water_Consumption_Api.Migrations
{
    /// <inheritdoc />
    public partial class initialcreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplianceConsumptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Appliance_Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Appliance_Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Daily_Usage = table.Column<double>(type: "REAL", nullable: true),
                    Night_Usage = table.Column<double>(type: "REAL", nullable: true),
                    Cost = table.Column<double>(type: "REAL", nullable: true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Daily_Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Night_Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Leakage_Percent = table.Column<double>(type: "REAL", nullable: true),
                    Account = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplianceConsumptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppliancePredictions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Appliance_Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Owner_AccountNumber = table.Column<string>(type: "TEXT", nullable: false),
                    ActualConsumptionKl = table.Column<double>(type: "REAL", nullable: false),
                    PredictedConsumptionKl = table.Column<double>(type: "REAL", nullable: false),
                    EstimatedCost = table.Column<double>(type: "REAL", nullable: false),
                    MonthYear = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppliancePredictions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Appliances",
                columns: table => new
                {
                    Appliance_Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Appliance_Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Appliance_MeterNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Owner_AccountNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    NumberOfPeopleUsingIt = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appliances", x => x.Appliance_Id);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Employee_Number = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    EmployeeFullNames = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    EmployeeEmail = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    EmployeePhone = table.Column<string>(type: "TEXT", maxLength: 15, nullable: false),
                    EmployeeAddress = table.Column<string>(type: "TEXT", maxLength: 250, nullable: true),
                    EmployeeRole = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    EmployeeIdNumber = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Employee_Number);
                });

            migrationBuilder.CreateTable(
                name: "HouseConsumptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Household_ID = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Owner_AccountNumber = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Water_Received = table.Column<double>(type: "REAL", nullable: true),
                    Total_WaterUsed = table.Column<double>(type: "REAL", nullable: true),
                    Leakage_Percent = table.Column<double>(type: "REAL", nullable: true),
                    Daily_Usage = table.Column<double>(type: "REAL", nullable: true),
                    Night_Usage = table.Column<double>(type: "REAL", nullable: true),
                    Spike_Usage = table.Column<double>(type: "REAL", nullable: true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Sub_RegionId = table.Column<int>(type: "INTEGER", nullable: true),
                    Extepected_Amount = table.Column<double>(type: "REAL", nullable: true),
                    Actual_Amount = table.Column<double>(type: "REAL", nullable: true),
                    OutStanding_Amount = table.Column<double>(type: "REAL", nullable: true),
                    FreeWater = table.Column<double>(type: "REAL", nullable: true),
                    Payment_Status = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HouseConsumptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HouseholdPrediction",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AccountNumber = table.Column<string>(type: "TEXT", nullable: false),
                    ActualConsumptionKl = table.Column<double>(type: "REAL", nullable: false),
                    PredictedConsumptionKl = table.Column<double>(type: "REAL", nullable: false),
                    EstimatedCost = table.Column<double>(type: "REAL", nullable: false),
                    MonthYear = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HouseholdPrediction", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Logins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Account_Number = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Staff_Id = table.Column<string>(type: "TEXT", maxLength: 6, nullable: true),
                    Email_Address = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Phone_Number = table.Column<string>(type: "TEXT", maxLength: 12, nullable: true),
                    Password = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Role = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    FullNames = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ModelTrainingPayloads",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ApplianceActive_Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Appliance_Name = table.Column<string>(type: "TEXT", nullable: false),
                    Account = table.Column<string>(type: "TEXT", nullable: false),
                    TimeItWasNotRunning = table.Column<double>(type: "REAL", nullable: false),
                    StartTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    StopedTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TotalDurationMinutes = table.Column<double>(type: "REAL", nullable: false),
                    PauseTimeMinutes = table.Column<double>(type: "REAL", nullable: false),
                    WaterUsed = table.Column<double>(type: "REAL", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    LeakageProbability = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelTrainingPayloads", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OwnerPayments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Account_Number = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Amount_Payed = table.Column<double>(type: "REAL", nullable: false),
                    Amount_Outstanding = table.Column<double>(type: "REAL", nullable: false),
                    Amount_Owing = table.Column<double>(type: "REAL", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OwnerPayments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Owners",
                columns: table => new
                {
                    Owner_IdNumber = table.Column<string>(type: "TEXT", maxLength: 13, nullable: false),
                    Owner_Name = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Owner_AccountNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Owner_MeterNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Owner_PhoneNumber = table.Column<string>(type: "TEXT", maxLength: 15, nullable: true),
                    Owner_Email = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Household_ID = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Sub_RegionId = table.Column<int>(type: "INTEGER", nullable: true),
                    Sub_RegionName = table.Column<string>(type: "TEXT", nullable: false),
                    PropertyValue = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Owners", x => x.Owner_IdNumber);
                });

            migrationBuilder.CreateTable(
                name: "RegionConsumptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Region_Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Region_Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Water_Sent = table.Column<double>(type: "REAL", nullable: true),
                    Water_Payed = table.Column<double>(type: "REAL", nullable: true),
                    OutstandingWater = table.Column<double>(type: "REAL", nullable: true),
                    Daily_Usage = table.Column<double>(type: "REAL", nullable: true),
                    Night_Usage = table.Column<double>(type: "REAL", nullable: true),
                    Leakage_Percent = table.Column<double>(type: "REAL", nullable: true),
                    Spike_Usage = table.Column<double>(type: "REAL", nullable: true),
                    Extepected_Amount = table.Column<double>(type: "REAL", nullable: true),
                    Actual_Amount = table.Column<double>(type: "REAL", nullable: true),
                    OutStanding_Amount = table.Column<double>(type: "REAL", nullable: true),
                    FreeWater = table.Column<double>(type: "REAL", nullable: true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegionConsumptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegionPrediction",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RegionId = table.Column<int>(type: "INTEGER", nullable: false),
                    RegionName = table.Column<string>(type: "TEXT", nullable: false),
                    ActualConsumptionKl = table.Column<double>(type: "REAL", nullable: false),
                    PredictedConsumptionKl = table.Column<double>(type: "REAL", nullable: false),
                    EstimatedCost = table.Column<double>(type: "REAL", nullable: false),
                    MonthYear = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegionPrediction", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Regions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Region_Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Region_Meter = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Regions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sub_RegionConsumptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Sub_RegionId = table.Column<int>(type: "INTEGER", nullable: false),
                    Sub_RegionName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    RegionId = table.Column<int>(type: "INTEGER", nullable: false),
                    Water_Sent = table.Column<double>(type: "REAL", nullable: true),
                    Water_Payed = table.Column<double>(type: "REAL", nullable: true),
                    OutstandingWater = table.Column<double>(type: "REAL", nullable: true),
                    Daily_Usage = table.Column<double>(type: "REAL", nullable: true),
                    Night_Usage = table.Column<double>(type: "REAL", nullable: true),
                    Leakage_Percent = table.Column<double>(type: "REAL", nullable: true),
                    Spike_Usage = table.Column<double>(type: "REAL", nullable: true),
                    Extepected_Amount = table.Column<double>(type: "REAL", nullable: true),
                    Actual_Amount = table.Column<double>(type: "REAL", nullable: true),
                    OutStanding_Amount = table.Column<double>(type: "REAL", nullable: true),
                    FreeWater = table.Column<double>(type: "REAL", nullable: true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sub_RegionConsumptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sub_Regions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SubRegionName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    RegionId = table.Column<int>(type: "INTEGER", nullable: true),
                    RegionName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    SubRegion_Meter = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sub_Regions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubRegionPrediction",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SubRegionId = table.Column<int>(type: "INTEGER", nullable: false),
                    SubRegionName = table.Column<string>(type: "TEXT", nullable: false),
                    ActualConsumptionKl = table.Column<double>(type: "REAL", nullable: false),
                    PredictedConsumptionKl = table.Column<double>(type: "REAL", nullable: false),
                    EstimatedCost = table.Column<double>(type: "REAL", nullable: false),
                    MonthYear = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubRegionPrediction", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplianceConsumptions");

            migrationBuilder.DropTable(
                name: "AppliancePredictions");

            migrationBuilder.DropTable(
                name: "Appliances");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "HouseConsumptions");

            migrationBuilder.DropTable(
                name: "HouseholdPrediction");

            migrationBuilder.DropTable(
                name: "Logins");

            migrationBuilder.DropTable(
                name: "ModelTrainingPayloads");

            migrationBuilder.DropTable(
                name: "OwnerPayments");

            migrationBuilder.DropTable(
                name: "Owners");

            migrationBuilder.DropTable(
                name: "RegionConsumptions");

            migrationBuilder.DropTable(
                name: "RegionPrediction");

            migrationBuilder.DropTable(
                name: "Regions");

            migrationBuilder.DropTable(
                name: "Sub_RegionConsumptions");

            migrationBuilder.DropTable(
                name: "Sub_Regions");

            migrationBuilder.DropTable(
                name: "SubRegionPrediction");
        }
    }
}
