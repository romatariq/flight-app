namespace App.Private.DTO.DAL;

public class Aircraft
{
    public Guid Id { get; set; }
    
    public string Icao { get; set; } = default!;
    
    public string? Registration { get; set; }

    public double? Latitude { get; set; }
    
    public double? Longitude { get; set; }

    public decimal SpeedKmh { get; set; }

    public string ModelName { get; set; } = default!;
    
    public DateTime InfoLastUpdatedUtc { get; set; }
}