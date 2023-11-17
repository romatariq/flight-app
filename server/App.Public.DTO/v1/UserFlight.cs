namespace App.Public.DTO.v1;

/// <summary>
/// User flight info.
/// </summary>
public class UserFlight
{
    /// <summary>
    /// Identification.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Flight identification.
    /// </summary>
    public Guid FlightId { get; set; }
    
    /// <summary>
    /// Iata code of flight (3 letter code).
    /// </summary>
    public string FlightIata { get; set; } = default!;
    
    /// <summary>
    /// Iata code of departure airport (3 letter code).
    /// </summary>
    public string DepartureAirportIata { get; set; } = default!;
    
    /// <summary>
    /// Iata code of arrival airport (3 letter code).
    /// </summary>
    public string ArrivalAirportIata { get; set; } = default!;
    
    /// <summary>
    /// Scheduled departure time in UTC.
    /// </summary>
    public DateTime ScheduledDepartureUtc { get; set; }
    
    /// <summary>
    /// Scheduled arrival time in UTC.
    /// </summary>
    public DateTime ScheduledArrivalUtc { get; set; }
    
    /// <summary>
    /// Scheduled departure time in local time of departure airport.
    /// </summary>
    public DateTime ScheduledDepartureLocal { get; set; }
    
    /// <summary>
    /// Scheduled arrival time in local time of arrival airport.
    /// </summary>
    public DateTime ScheduledArrivalLocal { get; set; }
}