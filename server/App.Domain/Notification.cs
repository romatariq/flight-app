using System.ComponentModel.DataAnnotations;
using Base.Domain;

namespace App.Domain;

public class Notification: DomainEntityId
{

    [MaxLength(100)]
    public string NotificationType { get; set; } = default!;

    
    public ICollection<UserFlightNotification>? UserFlightNotifications { get; set; }
}