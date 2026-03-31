namespace IndigoApi2.DTOs;

public class CreateWeatherEntry
{
    public string City { get; set; } = string.Empty;
    public double Temperature { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
}