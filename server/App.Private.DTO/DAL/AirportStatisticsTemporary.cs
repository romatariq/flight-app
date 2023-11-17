using App.Domain;

namespace App.Private.DTO.DAL;

public class AirportStatisticsTemporary
{
    public string AirportIata { get; set; } = default!;

    public string AirportName { get; set; } = default!;

    public IEnumerable<Flight> Departures { get; set; } = default!;
    
    public IEnumerable<Flight> Arrivals { get; set; } = default!;
}