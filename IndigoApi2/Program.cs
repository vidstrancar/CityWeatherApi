using IndigoApi2.Configuration;
using IndigoApi2.Middleware;
using IndigoApi2.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSingleton<WeatherService>();
builder.Services.AddControllers();

builder.Services.Configure<WeatherOptions>(
    builder.Configuration.GetSection(WeatherOptions.SectionName));

var app = builder.Build();

var weatherService = app.Services.GetRequiredService<WeatherService>();
_ = Task.Run(async () => 
{
    try 
    {
        await weatherService.ProcessDataAsync();
    }
    catch (Exception ex) 
    {
        // This will catch path errors, null options, etc.
        Console.WriteLine($"FATAL ERROR in Background Task: {ex.Message}");
    }
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseMiddleware<ApiKeyMiddleware>();
app.MapControllers();

app.Run();