using Base.Domain;

namespace App.Private.DTO.DAL;

public class UserNotification: DomainEntityId
{
    public string NotificationType { get; set; } = default!;

    public string UserEmail { get; set; } = default!;
    
    public string UserFirstName { get; set; } = default!;

    public int MinutesFromEvent { get; set; }

    public string FlightIata { get; set; } = default!;

    public string DepartureAirportName { get; set; } = default!;
    
    public string ArrivalAirportName { get; set; } = default!;
}