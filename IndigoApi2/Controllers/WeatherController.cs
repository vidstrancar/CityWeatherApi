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
}