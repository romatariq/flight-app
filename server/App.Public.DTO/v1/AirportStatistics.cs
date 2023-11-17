namespace App.Public.DTO.v1;

/// <summary>
/// Airport statistics.
/// </summary>
public class AirportStatistics
{
    /// <summary>
    /// Name of the airport statistics is about.
    /// </summary>
    public string AirportName { get; set; } = default!;
    
    /// <summary>
    /// IATA code of the airport statistics is about.
    /// </summary>
    public string AirportIata { get; set; } = default!;
    
    /// <summary>
    /// Total number of departures in given period.
    /// </summary>
    public int DeparturesCount { get; set; }
    
    /// <summary>
    /// Total number of arrivals in given period.
    /// </summary>
    public int ArrivalsCount { get; set; }

    
    /// <summary>
    /// Airline frequency list for departures.
    /// </summary>
    public IEnumerable<NameCounter> DepartureAirlines { get; set; } = default!;
    
    /// <summary>
    /// Airline frequency list for arrivals.
    /// </summary>
    public IEnumerable<NameCounter> ArrivalAirlines { get; set; } = default!;
    
    /// <summary>
    /// Country frequency list for departures.
    /// </summary>
    public IEnumerable<NameCounter> DepartureCountries { get; set; } = default!;
    
    /// <summary>
    /// Country frequency list for arrivals.
    /// </summary>
    public IEnumerable<NameCounter> ArrivalCountries { get; set; } = default!;
}