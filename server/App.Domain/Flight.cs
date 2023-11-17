using System.ComponentModel.DataAnnotations;
using Base.Domain;

namespace App.Domain;

public class Flight: DomainEntityId
{

    [MaxLength(10)]
    public string FlightIata { get; set; } = default!;

    public DateTime ScheduledDepartureUtc { get; set; }
    public DateTime ScheduledArrivalUtc { get; set; }

    public DateTime ExpectedDepartureUtc { get; set; }
    public DateTime ExpectedArrivalUtc { get; set; }

    [MaxLength(30)]
    public string? DepartureTerminal { get; set; }
        
    [MaxLength(30)]
    public string? ArrivalTerminal { get; set; }
    
    [MaxLength(30)]
    public string? DepartureGate { get; set; }

    [MaxLength(30)]
    public string? ArrivalGate { get; set; }

    public DateTime? FlightInfoLastCheckedUtc { get; set; }
    
    
    public Guid DepartureAirportId { get; set; }
    public Airport? DepartureAirport { get; set; }

    public Guid ArrivalAirportId { get; set; }
    public Airport? ArrivalAirport { get; set; }

    public Guid FlightStatusId { get; set; }
    public FlightStatus? FlightStatus { get; set; }

    public Guid AirlineId { get; set; }
    public Airline? Airline { get; set; }

    public Guid? AircraftId { get; set; }
    public Aircraft? Aircraft { get; set; }

    public ICollection<UserFlight>? UserFlights { get; set; }
}