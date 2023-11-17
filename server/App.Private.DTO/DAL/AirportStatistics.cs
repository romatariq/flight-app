namespace App.Private.DTO.DAL;

public class AirportStatistics
{
    public string AirportName { get; set; } = default!;
    
    public string AirportIata { get; set; } = default!;
    
    public int DeparturesCount { get; set; }
    
    public int ArrivalsCount { get; set; }

    public IEnumerable<NameCounter> DepartureAirlines { get; set; } = default!;
    public IEnumerable<NameCounter> ArrivalAirlines { get; set; } = default!;
    
    public IEnumerable<NameCounter> DepartureCountries { get; set; } = default!;
    public IEnumerable<NameCounter> ArrivalCountries { get; set; } = default!;
}