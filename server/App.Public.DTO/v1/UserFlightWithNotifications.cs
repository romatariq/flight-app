namespace App.Public.DTO.v1;

/// <summary>
/// Info required for user flight notification page.
/// </summary>
public class UserFlightWithNotifications
{
    /// <summary>
    /// UserFlight identification.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Departure airport name.
    /// </summary>
    public string DepartureAirportName { get; set; } = default!;
    
    /// <summary>
    /// Arrival airport name.
    /// </summary>
    public string ArrivalAirportName { get; set; } = default!;

    /// <summary>
    /// Iata code of departure airport.
    /// </summary>
    public string DepartureAirportIata { get; set; } = default!;
    
    /// <summary>
    /// Iata code of arrival airport.
    /// </summary>
    public string ArrivalAirportIata { get; set; } = default!;

    /// <summary>
    /// Flight iata code.
    /// </summary>
    public string FlightIata { get; set; } = default!;

    /// <summary>
    /// List of user notifications for this flight.
    /// </summary>
    public IEnumerable<UserNotification> UserNotifications { get; set; } = default!;
    
    /// <summary>
    /// List of all possible notification types.
    /// </summary>
    public IEnumerable<Notification> AllNotificationTypes { get; set; } = default!;
}