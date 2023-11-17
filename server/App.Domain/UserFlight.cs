using App.Domain.Identity;
using Base.Domain;

namespace App.Domain;

public class UserFlight: DomainEntityId
{
    
    public bool NotifyGateChanges { get; set; }
    

    public Guid FlightId { get; set; }
    public Flight? Flight { get; set; }

    public Guid AppUserId { get; set; }
    public AppUser? AppUser { get; set; }
    

    public ICollection<UserFlightNotification>? UserFlightNotifications { get; set; }
}