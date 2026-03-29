using IndigoApi2.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSingleton<WeatherService>();
builder.Services.AddControllers();

var app = builder.Build();

var weatherService = app.Services.GetRequiredService<WeatherService>();
_ = Task.Run(() => weatherService.ProcessDataAsync());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();