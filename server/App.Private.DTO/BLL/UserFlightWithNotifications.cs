using Base.Domain;

namespace App.Private.DTO.BLL;

public class UserFlightWithNotifications: DomainEntityId
{
    // Id is user flight id

    public string DepartureAirportName { get; set; } = default!;
    
    public string ArrivalAirportName { get; set; } = default!;

    public string DepartureAirportIata { get; set; } = default!;
    
    public string ArrivalAirportIata { get; set; } = default!;

    public string FlightIata { get; set; } = default!;

    public IEnumerable<UserFlightNotificationInfo> UserNotifications { get; set; } = default!;
    
    public IEnumerable<Notification> AllNotificationTypes { get; set; } = default!;
}