using Base.Domain;

namespace App.Private.DTO.BLL;

public class UserFlightNotificationInfo: DomainEntityId
{
    public Guid UserId { get; set; }

    public Guid NotificationId { get; set; }

    public string NotificationType { get; set; } = default!;

    public int MinutesFromEvent { get; set; }
}