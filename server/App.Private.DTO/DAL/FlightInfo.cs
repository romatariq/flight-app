using App.Private.DTO.IDTOs;
using Base.Domain;

namespace App.Private.DTO.DAL;

public class FlightInfo: DomainEntityId, IAirportsCoordinates
{
    public string FlightIata { get; set; } = default!;
    
    public DateTime ScheduledDepartureUtc { get; set; }
    public DateTime ScheduledArrivalUtc { get; set; }

    public string Airline { get; set; } = default!;

    public string DepartureAirportName { get; set; } = default!;
    public string ArrivalAirportName { get; set; } = default!;

    public string DepartureAirportIata { get; set; } = default!;
    public string ArrivalAirportIata { get; set; } = default!;
    
    public double DepartureAirportLatitude { get; set; }
    public double DepartureAirportLongitude { get; set; }
    
    public double ArrivalAirportLatitude { get; set; }
    public double ArrivalAirportLongitude { get; set; }

    public string Status { get; set; } = default!;
}