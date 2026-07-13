using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Smart_Water_Consumption_Api.Migrations
{
    /// <inheritdoc />
    public partial class mg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModelTrainingPayloads");

            migrationBuilder.RenameColumn(
                name: "PredictedConsumptionKl",
                table: "SubRegionPrediction",
                newName: "PredictedWeekConsumptionKl");

            migrationBuilder.RenameColumn(
                name: "EstimatedCost",
                table: "SubRegionPrediction",
                newName: "PredictedMonthConsumptionKl");

            migrationBuilder.RenameColumn(
                name: "ActualConsumptionKl",
                table: "SubRegionPrediction",
                newName: "PredictedHourConsumptionKl");

            migrationBuilder.RenameColumn(
                name: "PredictedConsumptionKl",
                table: "RegionPrediction",
                newName: "PredictedWeekConsumptionKl");

            migrationBuilder.RenameColumn(
                name: "EstimatedCost",
                table: "RegionPrediction",
                newName: "PredictedMonthConsumptionKl");

            migrationBuilder.RenameColumn(
                name: "ActualConsumptionKl",
                table: "RegionPrediction",
                newName: "PredictedHourConsumptionKl");

            migrationBuilder.RenameColumn(
                name: "PredictedConsumptionKl",
                table: "HouseholdPrediction",
                newName: "PredictedWeekConsumptionKl");

            migrationBuilder.RenameColumn(
                name: "EstimatedCost",
                table: "HouseholdPrediction",
                newName: "PredictedMonthConsumptionKl");

            migrationBuilder.RenameColumn(
                name: "ActualConsumptionKl",
                table: "HouseholdPrediction",
                newName: "PredictedHourConsumptionKl");

            migrationBuilder.RenameColumn(
                name: "PredictedConsumptionKl",
                table: "AppliancePredictions",
                newName: "PredictedWeekConsumptionKl");

            migrationBuilder.RenameColumn(
                name: "EstimatedCost",
                table: "AppliancePredictions",
                newName: "PredictedMonthConsumptionKl");

            migrationBuilder.RenameColumn(
                name: "ActualConsumptionKl",
                table: "AppliancePredictions",
                newName: "PredictedHourConsumptionKl");

            migrationBuilder.AddColumn<double>(
                name: "ActualMonthConsumptionKl",
                table: "SubRegionPrediction",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "EstimatedHourCost",
                table: "SubRegionPrediction",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "EstimatedMonthCost",
                table: "SubRegionPrediction",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "EstimatedWeekCost",
                table: "SubRegionPrediction",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ActualMonthConsumptionKl",
                table: "RegionPrediction",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "EstimatedHourCost",
                table: "RegionPrediction",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "EstimatedMonthCost",
                table: "RegionPrediction",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "EstimatedWeekCost",
                table: "RegionPrediction",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ActualHourConsumptionKl",
                table: "HouseholdPrediction",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ActualMonthConsumptionKl",
                table: "HouseholdPrediction",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "EstimatedHourCost",
                table: "HouseholdPrediction",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "EstimatedMonthCost",
                table: "HouseholdPrediction",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "EstimatedWeekCost",
                table: "HouseholdPrediction",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ActualMonthConsumptionKl",
                table: "AppliancePredictions",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "EstimatedHourCost",
                table: "AppliancePredictions",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "EstimatedMonthCost",
                table: "AppliancePredictions",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "EstimatedWeekCost",
                table: "AppliancePredictions",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualMonthConsumptionKl",
                table: "SubRegionPrediction");

            migrationBuilder.DropColumn(
                name: "EstimatedHourCost",
                table: "SubRegionPrediction");

            migrationBuilder.DropColumn(
                name: "EstimatedMonthCost",
                table: "SubRegionPrediction");

            migrationBuilder.DropColumn(
                name: "EstimatedWeekCost",
                table: "SubRegionPrediction");

            migrationBuilder.DropColumn(
                name: "ActualMonthConsumptionKl",
                table: "RegionPrediction");

            migrationBuilder.DropColumn(
                name: "EstimatedHourCost",
                table: "RegionPrediction");

            migrationBuilder.DropColumn(
                name: "EstimatedMonthCost",
                table: "RegionPrediction");

            migrationBuilder.DropColumn(
                name: "EstimatedWeekCost",
                table: "RegionPrediction");

            migrationBuilder.DropColumn(
                name: "ActualHourConsumptionKl",
                table: "HouseholdPrediction");

            migrationBuilder.DropColumn(
                name: "ActualMonthConsumptionKl",
                table: "HouseholdPrediction");

            migrationBuilder.DropColumn(
                name: "EstimatedHourCost",
                table: "HouseholdPrediction");

            migrationBuilder.DropColumn(
                name: "EstimatedMonthCost",
                table: "HouseholdPrediction");

            migrationBuilder.DropColumn(
                name: "EstimatedWeekCost",
                table: "HouseholdPrediction");

            migrationBuilder.DropColumn(
                name: "ActualMonthConsumptionKl",
                table: "AppliancePredictions");

            migrationBuilder.DropColumn(
                name: "EstimatedHourCost",
                table: "AppliancePredictions");

            migrationBuilder.DropColumn(
                name: "EstimatedMonthCost",
                table: "AppliancePredictions");

            migrationBuilder.DropColumn(
                name: "EstimatedWeekCost",
                table: "AppliancePredictions");

            migrationBuilder.RenameColumn(
                name: "PredictedWeekConsumptionKl",
                table: "SubRegionPrediction",
                newName: "PredictedConsumptionKl");

            migrationBuilder.RenameColumn(
                name: "PredictedMonthConsumptionKl",
                table: "SubRegionPrediction",
                newName: "EstimatedCost");

            migrationBuilder.RenameColumn(
                name: "PredictedHourConsumptionKl",
                table: "SubRegionPrediction",
                newName: "ActualConsumptionKl");

            migrationBuilder.RenameColumn(
                name: "PredictedWeekConsumptionKl",
                table: "RegionPrediction",
                newName: "PredictedConsumptionKl");

            migrationBuilder.RenameColumn(
                name: "PredictedMonthConsumptionKl",
                table: "RegionPrediction",
                newName: "EstimatedCost");

            migrationBuilder.RenameColumn(
                name: "PredictedHourConsumptionKl",
                table: "RegionPrediction",
                newName: "ActualConsumptionKl");

            migrationBuilder.RenameColumn(
                name: "PredictedWeekConsumptionKl",
                table: "HouseholdPrediction",
                newName: "PredictedConsumptionKl");

            migrationBuilder.RenameColumn(
                name: "PredictedMonthConsumptionKl",
                table: "HouseholdPrediction",
                newName: "EstimatedCost");

            migrationBuilder.RenameColumn(
                name: "PredictedHourConsumptionKl",
                table: "HouseholdPrediction",
                newName: "ActualConsumptionKl");

            migrationBuilder.RenameColumn(
                name: "PredictedWeekConsumptionKl",
                table: "AppliancePredictions",
                newName: "PredictedConsumptionKl");

            migrationBuilder.RenameColumn(
                name: "PredictedMonthConsumptionKl",
                table: "AppliancePredictions",
                newName: "EstimatedCost");

            migrationBuilder.RenameColumn(
                name: "PredictedHourConsumptionKl",
                table: "AppliancePredictions",
                newName: "ActualConsumptionKl");

            migrationBuilder.CreateTable(
                name: "ModelTrainingPayloads",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Account = table.Column<string>(type: "TEXT", nullable: false),
                    ApplianceActive_Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Appliance_Name = table.Column<string>(type: "TEXT", nullable: false),
                    LeakageProbability = table.Column<double>(type: "REAL", nullable: false),
                    PauseTimeMinutes = table.Column<double>(type: "REAL", nullable: false),
                    StartTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    StopedTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TimeItWasNotRunning = table.Column<double>(type: "REAL", nullable: false),
                    TotalDurationMinutes = table.Column<double>(type: "REAL", nullable: false),
                    WaterUsed = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelTrainingPayloads", x => x.Id);
                });
        }
    }
}
