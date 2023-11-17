using Base.Domain;

namespace App.Private.DTO.BLL;

public class FlightInfo: DomainEntityId
{
    public string FlightIata { get; set; } = default!;

    public DateTime ScheduledDepartureLocal { get; set; }
    public DateTime ScheduledArrivalLocal { get; set; }
    
    public DateTime ScheduledDepartureUtc { get; set; }
    public DateTime ScheduledArrivalUtc { get; set; }

    public string Airline { get; set; } = default!;
    
    public string DepartureAirportIata { get; set; } = default!;
    public string ArrivalAirportIata { get; set; } = default!;
    
    public string DepartureAirportName { get; set; } = default!;
    public string ArrivalAirportName { get; set; } = default!;

    public string Status { get; set; } = default!;
}