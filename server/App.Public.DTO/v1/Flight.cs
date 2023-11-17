namespace App.Public.DTO.v1;

/// <summary>
/// Flight info.
/// </summary>
public class Flight
{
    /// <summary>
    /// Identification.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Iata (3 letter code) of flight.
    /// </summary>
    public string FlightIata { get; set; } = default!;

    /// <summary>
    /// Scheduled departure date and time in departure airport local time.
    /// </summary>
    public DateTime ScheduledDepartureLocal { get; set; }
    
    /// <summary>
    /// Scheduled arrival date and time in arrival airport local time.
    /// </summary>
    public DateTime ScheduledArrivalLocal { get; set; }
    
    /// <summary>
    /// Scheduled departure date and time in UTC.
    /// </summary>
    public DateTime ScheduledDepartureUtc { get; set; }
    
    /// <summary>
    /// Scheduled arrival date and time in UTC.
    /// </summary>
    public DateTime ScheduledArrivalUtc { get; set; }

    /// <summary>
    /// Airline name.
    /// </summary>
    public string Airline { get; set; } = default!;
    
    /// <summary>
    /// Iata (3 letter code) of departure airport.
    /// </summary>
    public string DepartureAirportIata { get; set; } = default!;
    
    /// <summary>
    /// Iata (3 letter code) of arrival airport.
    /// </summary>
    public string ArrivalAirportIata { get; set; } = default!;
    
    /// <summary>
    /// Departure airport name.
    /// </summary>
    public string DepartureAirportName { get; set; } = default!;
    
    /// <summary>
    /// Arrival airport name.
    /// </summary>
    public string ArrivalAirportName { get; set; } = default!;

    /// <summary>
    /// Flight status name.
    /// </summary>
    public string Status { get; set; } = default!;
}