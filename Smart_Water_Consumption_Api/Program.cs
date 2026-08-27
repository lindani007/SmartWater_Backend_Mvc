using Microsoft.EntityFrameworkCore;
using Smart_Water_Consumption_Api.Ai.MachineLearningModel;
using Smart_Water_Consumption_Api.Hubs;
using Smart_Water_Consumption_Api.Models.Db;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ModelsPrediction>();

builder.Services.AddDbContext<SmartWaterCnsumptionDbContentext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DropWiseDb");
        options.UseSqlServer(connectionString);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("SignalRPolicy", policy =>
    {
        policy.WithOrigins("https://localhost:7164") // Your exact MVC port from the screenshot
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Mandated by SignalR for sticky sessions/WebSockets
    });
});
builder.Services.AddSignalR();
var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseCors("SignalRPolicy");
app.MapHub<DashboardRealtimeUpdateHub>("/dashboardHub");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
