using IndigoApi2.DTOs;
using IndigoApi2.Services;
using Microsoft.AspNetCore.Mvc;

namespace IndigoApi2.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherController : ControllerBase
{
    private readonly WeatherService _service;

    public WeatherController(WeatherService service) => _service = service;

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_service.GetAll());
    }

    [HttpGet("{city}")]
    public IActionResult GetCity(string city) 
    {
        var result = _service.GetByCity(city);
        return result != null ? Ok(result) : NotFound();
    }

    [HttpGet("filter")]
    public IActionResult GetFiltered([FromQuery] double? minAvg, [FromQuery] double? maxAvg)
    {
        var query = _service.GetAll();
        if (minAvg.HasValue) query = query.Where(c => c.Average >= minAvg.Value);
        if (maxAvg.HasValue) query = query.Where(c => c.Average <= maxAvg.Value);
        
        return Ok(query.ToList());
    }

    [HttpPost("recalculate")]
    public async Task<IActionResult> Recalculate()
    {
        // Fire and forget or await depending on your UI needs
        _ = Task.Run(() => _service.ProcessDataAsync());
        return Accepted("Recalculation started...");
    }
    
    [HttpPost("create weather entry")]
    public async Task<IActionResult> Post([FromBody] CreateWeatherEntry entry)
    {
        if (string.IsNullOrWhiteSpace(entry.City))
            return BadRequest("City name is required.");
        
        if (entry.City.Contains(';')) 
            return BadRequest("City name cannot contain semicolons.");

        await _service.AddEntryAsync(entry);
    
        // Return the updated stats for that specific city
        return CreatedAtAction(
            nameof(GetCity), 
            new { city = entry.City }, 
            _service.GetByCity(entry.City));
    }
}