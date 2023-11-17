using Base.Domain;

namespace App.Domain;

public class UserFlightNotification: DomainEntityId
{

    public int MinutesFromEvent { get; set; }

    
    public Guid UserFlightId { get; set; }
    public UserFlight? UserFlight { get; set; }

    public Guid NotificationId { get; set; }
    public Notification? Notification { get; set; }
}