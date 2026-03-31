namespace IndigoApi2.DTOs;

public class CityWeatherResponse
{
    public string City { get; set; } = string.Empty;
    public double Min { get; set; }
    public double Max { get; set; }
    public double Average { get; set; }
}