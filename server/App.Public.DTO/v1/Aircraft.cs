namespace App.Public.DTO.v1;

/// <summary>
/// Aircraft info.
/// </summary>
public class Aircraft
{
    /// <summary>
    /// Identification.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Icao (Hexadecimal code) of aircraft.
    /// </summary>
    public string Icao { get; set; } = default!;
    
    /// <summary>
    /// Registration number of aircraft.
    /// </summary>
    public string? Registration { get; set; }

    /// <summary>
    /// Latitude position of aircraft.
    /// </summary>
    public double? Latitude { get; set; }
    
    /// <summary>
    /// Longitude position of aircraft.
    /// </summary>
    public double? Longitude { get; set; }

    /// <summary>
    /// Speed of aircraft in km/h.
    /// </summary>
    public decimal SpeedKmh { get; set; }

    /// <summary>
    /// Name of aircraft's model.
    /// </summary>
    public string ModelName { get; set; } = default!;
}