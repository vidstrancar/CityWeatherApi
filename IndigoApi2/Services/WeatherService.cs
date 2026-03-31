using IndigoApi2.Configuration;
using IndigoApi2.DTOs;
using IndigoApi2.Models;
using Microsoft.Extensions.Options;

namespace IndigoApi2.Services;

using System.Collections.Concurrent;
using System.Globalization;

public class WeatherService(IOptions<WeatherOptions> options)
{
    private ConcurrentDictionary<string, CityStats> _cache = new();
    private readonly string _filePath = options.Value.MeasurementFilePath ?? 
                                        throw new ArgumentNullException(nameof(options));

    public async Task ProcessDataAsync()
    {
        Console.WriteLine($"Processing data: {_filePath}");
        
        var newStats = new ConcurrentDictionary<string, CityStats>();

        // Use this inside ProcessDataAsync to allow Appends while Reading
        await using var stream = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var reader = new StreamReader(stream);
        while (await reader.ReadLineAsync() is { } line)
        {
            
            var parts = line.Split(';');
            if (parts.Length < 3) continue;

            string cityName = parts[1];
            if (!double.TryParse(parts[2], NumberStyles.Any, CultureInfo.InvariantCulture, out double temp)) 
                continue;

            newStats.AddOrUpdate(cityName, 
                // Add new entry
                _ => new CityStats { City = cityName, Min = temp, Max = temp, TotalSum = temp, Count = 1 },
                // Update existing entry
                (_, existing) => {
                    existing.Min = Math.Min(existing.Min, temp);
                    existing.Max = Math.Max(existing.Max, temp);
                    existing.TotalSum += temp;
                    existing.Count++;
                    return existing;
                });
        }
        _cache = newStats; // Atomic swap of the "cache"
        
        Console.WriteLine($"Finished processing data");
    }
    
    public async Task AddEntryAsync(CreateWeatherEntry entry)
    {
        // 1. Format the line for the CSV (Date;City;Temp)
        // Use InvariantCulture to ensure decimals use '.' and not ','
        string line = $"{entry.Date:yyyy-MM-dd};{entry.City};{entry.Temperature.ToString(CultureInfo.InvariantCulture)}\n";

        // 2. Append to the file (Async & Thread-safe for writing)
        await File.AppendAllTextAsync(_filePath, line);

        // 3. Update the Cache incrementally
        _cache.AddOrUpdate(entry.City,
            // If city doesn't exist yet
            _ => new CityStats { 
                City = entry.City, 
                Min = entry.Temperature, 
                Max = entry.Temperature, 
                TotalSum = entry.Temperature, 
                Count = 1 
            },
            // If city exists, update the aggregates
            (_, existing) => {
                existing.Min = Math.Min(existing.Min, entry.Temperature);
                existing.Max = Math.Max(existing.Max, entry.Temperature);
                existing.TotalSum += entry.Temperature;
                existing.Count++;
                return existing;
            });
    }

    public IEnumerable<CityStats> GetAll() => _cache.Values;
    public CityStats? GetByCity(string city) => _cache.GetValueOrDefault(city);
}