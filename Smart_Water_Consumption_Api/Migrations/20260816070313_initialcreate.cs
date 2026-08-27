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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Appliance_Id = table.Column<int>(type: "int", nullable: false),
                    Appliance_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Daily_Usage = table.Column<double>(type: "float", nullable: true),
                    Night_Usage = table.Column<double>(type: "float", nullable: true),
                    Cost = table.Column<double>(type: "float", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Daily_Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Night_Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Leakage_Percent = table.Column<double>(type: "float", nullable: true),
                    Account = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplianceConsumptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppliancePredictions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Appliance_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Owner_AccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PredictedHourConsumptionKl = table.Column<double>(type: "float", nullable: false),
                    PredictedWeekConsumptionKl = table.Column<double>(type: "float", nullable: false),
                    ActualMonthConsumptionKl = table.Column<double>(type: "float", nullable: false),
                    PredictedMonthConsumptionKl = table.Column<double>(type: "float", nullable: false),
                    EstimatedHourCost = table.Column<double>(type: "float", nullable: false),
                    EstimatedWeekCost = table.Column<double>(type: "float", nullable: false),
                    EstimatedMonthCost = table.Column<double>(type: "float", nullable: false),
                    MonthYear = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppliancePredictions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Appliances",
                columns: table => new
                {
                    Appliance_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Appliance_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Appliance_MeterNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Owner_AccountNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NumberOfPeopleUsingIt = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appliances", x => x.Appliance_Id);
                });

            migrationBuilder.CreateTable(
                name: "ChatMessages",
                columns: table => new
                {
                    ChatId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChatTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessages", x => x.ChatId);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Employee_Number = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EmployeeFullNames = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    EmployeeEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EmployeePhone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    EmployeeAddress = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    EmployeeRole = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    EmployeeIdNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Employee_Number);
                });

            migrationBuilder.CreateTable(
                name: "HouseConsumptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Household_ID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Owner_AccountNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Water_Received = table.Column<double>(type: "float", nullable: true),
                    Total_WaterUsed = table.Column<double>(type: "float", nullable: true),
                    Leakage_Percent = table.Column<double>(type: "float", nullable: true),
                    Daily_Usage = table.Column<double>(type: "float", nullable: true),
                    Night_Usage = table.Column<double>(type: "float", nullable: true),
                    Spike_Usage = table.Column<double>(type: "float", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Sub_RegionId = table.Column<int>(type: "int", nullable: true),
                    Extepected_Amount = table.Column<double>(type: "float", nullable: true),
                    Actual_Amount = table.Column<double>(type: "float", nullable: true),
                    OutStanding_Amount = table.Column<double>(type: "float", nullable: true),
                    FreeWater = table.Column<double>(type: "float", nullable: true),
                    Payment_Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HouseConsumptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HouseholdPrediction",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubRegionId = table.Column<int>(type: "int", nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActualHourConsumptionKl = table.Column<double>(type: "float", nullable: false),
                    PredictedHourConsumptionKl = table.Column<double>(type: "float", nullable: false),
                    PredictedWeekConsumptionKl = table.Column<double>(type: "float", nullable: false),
                    ActualMonthConsumptionKl = table.Column<double>(type: "float", nullable: false),
                    PredictedMonthConsumptionKl = table.Column<double>(type: "float", nullable: false),
                    EstimatedHourCost = table.Column<double>(type: "float", nullable: false),
                    EstimatedWeekCost = table.Column<double>(type: "float", nullable: false),
                    EstimatedMonthCost = table.Column<double>(type: "float", nullable: false),
                    MonthYear = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HouseholdPrediction", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Logins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Account_Number = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Staff_Id = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: true),
                    Email_Address = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Phone_Number = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: true),
                    Password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    FullNames = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MessageOfUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChatId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsUser = table.Column<bool>(type: "bit", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageOfUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OwnerPayments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Account_Number = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Amount_Payed = table.Column<double>(type: "float", nullable: false),
                    Amount_Outstanding = table.Column<double>(type: "float", nullable: false),
                    Amount_Owing = table.Column<double>(type: "float", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OwnerPayments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Owners",
                columns: table => new
                {
                    Owner_IdNumber = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    Owner_Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Owner_AccountNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Owner_MeterNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Owner_PhoneNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Owner_Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Household_ID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Sub_RegionId = table.Column<int>(type: "int", nullable: true),
                    Sub_RegionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PropertyValue = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Owners", x => x.Owner_IdNumber);
                });

            migrationBuilder.CreateTable(
                name: "RegionConsumptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Region_Id = table.Column<int>(type: "int", nullable: false),
                    Region_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Water_Sent = table.Column<double>(type: "float", nullable: true),
                    Water_Payed = table.Column<double>(type: "float", nullable: true),
                    OutstandingWater = table.Column<double>(type: "float", nullable: true),
                    Daily_Usage = table.Column<double>(type: "float", nullable: true),
                    Night_Usage = table.Column<double>(type: "float", nullable: true),
                    Leakage_Percent = table.Column<double>(type: "float", nullable: true),
                    Spike_Usage = table.Column<double>(type: "float", nullable: true),
                    Extepected_Amount = table.Column<double>(type: "float", nullable: true),
                    Actual_Amount = table.Column<double>(type: "float", nullable: true),
                    OutStanding_Amount = table.Column<double>(type: "float", nullable: true),
                    FreeWater = table.Column<double>(type: "float", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegionConsumptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegionPrediction",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RegionId = table.Column<int>(type: "int", nullable: false),
                    RegionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PredictedHourConsumptionKl = table.Column<double>(type: "float", nullable: false),
                    PredictedWeekConsumptionKl = table.Column<double>(type: "float", nullable: false),
                    ActualMonthConsumptionKl = table.Column<double>(type: "float", nullable: false),
                    PredictedMonthConsumptionKl = table.Column<double>(type: "float", nullable: false),
                    EstimatedHourCost = table.Column<double>(type: "float", nullable: false),
                    EstimatedWeekCost = table.Column<double>(type: "float", nullable: false),
                    EstimatedMonthCost = table.Column<double>(type: "float", nullable: false),
                    MonthYear = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegionPrediction", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Regions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Region_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Region_Meter = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Regions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sub_RegionConsumptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sub_RegionId = table.Column<int>(type: "int", nullable: false),
                    Sub_RegionName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RegionId = table.Column<int>(type: "int", nullable: false),
                    Water_Sent = table.Column<double>(type: "float", nullable: true),
                    Water_Payed = table.Column<double>(type: "float", nullable: true),
                    OutstandingWater = table.Column<double>(type: "float", nullable: true),
                    Daily_Usage = table.Column<double>(type: "float", nullable: true),
                    Night_Usage = table.Column<double>(type: "float", nullable: true),
                    Leakage_Percent = table.Column<double>(type: "float", nullable: true),
                    Spike_Usage = table.Column<double>(type: "float", nullable: true),
                    Extepected_Amount = table.Column<double>(type: "float", nullable: true),
                    Actual_Amount = table.Column<double>(type: "float", nullable: true),
                    OutStanding_Amount = table.Column<double>(type: "float", nullable: true),
                    FreeWater = table.Column<double>(type: "float", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sub_RegionConsumptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sub_Regions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubRegionName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RegionId = table.Column<int>(type: "int", nullable: true),
                    RegionName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SubRegion_Meter = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sub_Regions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubRegionPrediction",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubRegionId = table.Column<int>(type: "int", nullable: false),
                    RegionId = table.Column<int>(type: "int", nullable: false),
                    SubRegionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PredictedHourConsumptionKl = table.Column<double>(type: "float", nullable: false),
                    PredictedWeekConsumptionKl = table.Column<double>(type: "float", nullable: false),
                    ActualMonthConsumptionKl = table.Column<double>(type: "float", nullable: false),
                    PredictedMonthConsumptionKl = table.Column<double>(type: "float", nullable: false),
                    EstimatedHourCost = table.Column<double>(type: "float", nullable: false),
                    EstimatedWeekCost = table.Column<double>(type: "float", nullable: false),
                    EstimatedMonthCost = table.Column<double>(type: "float", nullable: false),
                    MonthYear = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                name: "ChatMessages");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "HouseConsumptions");

            migrationBuilder.DropTable(
                name: "HouseholdPrediction");

            migrationBuilder.DropTable(
                name: "Logins");

            migrationBuilder.DropTable(
                name: "MessageOfUsers");

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
