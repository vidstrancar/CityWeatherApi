namespace IndigoApi2.Configuration;

public class WeatherOptions
{
    public const string SectionName = "WeatherSettings";
    public string MeasurementFilePath { get; set; } = string.Empty;
}