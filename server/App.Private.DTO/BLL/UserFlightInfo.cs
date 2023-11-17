using App.Private.DTO.IDTOs;
using Base.Domain;

namespace App.Private.DTO.BLL;

public class UserFlightInfo: DomainEntityId, IAirportsCoordinates
{
    public Guid FlightId { get; set; }
    
    public string FlightIata { get; set; } = default!;
    
    public string DepartureAirportIata { get; set; } = default!;
    public string ArrivalAirportIata { get; set; } = default!;
    
    public DateTime ScheduledDepartureUtc { get; set; }
    public DateTime ScheduledArrivalUtc { get; set; }

    public double DepartureAirportLatitude { get; set; }
    public double DepartureAirportLongitude { get; set; }
    
    public double ArrivalAirportLatitude { get; set; }
    public double ArrivalAirportLongitude { get; set; }
    
    public DateTime ScheduledDepartureLocal { get; set; }
    public DateTime ScheduledArrivalLocal { get; set; }
}