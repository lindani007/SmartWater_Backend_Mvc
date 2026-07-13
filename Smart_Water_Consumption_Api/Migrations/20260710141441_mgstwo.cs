using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Smart_Water_Consumption_Api.Migrations
{
    /// <inheritdoc />
    public partial class mgstwo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RegionId",
                table: "SubRegionPrediction",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SubRegionId",
                table: "HouseholdPrediction",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RegionId",
                table: "SubRegionPrediction");

            migrationBuilder.DropColumn(
                name: "SubRegionId",
                table: "HouseholdPrediction");
        }
    }
}
