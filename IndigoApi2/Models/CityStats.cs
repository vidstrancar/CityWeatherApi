namespace IndigoApi2.Models;

public class CityStats
{
    public string City { get; set; } = string.Empty;
    public double Min { get; set; } = double.MaxValue;
    public double Max { get; set; } = double.MinValue;
    public double TotalSum { get; set; }
    public long Count { get; set; }
    public double Average => Count > 0 ? TotalSum / Count : 0;
}