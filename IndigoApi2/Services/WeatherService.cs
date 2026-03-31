using IndigoApi2.Configuration;
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

        // Use a StreamReader to process the file line-by-line
        using var reader = new StreamReader(_filePath);
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

    public IEnumerable<CityStats> GetAll() => _cache.Values;
    public CityStats? GetByCity(string city) => _cache.TryGetValue(city, out var stats) ? stats : null;
}