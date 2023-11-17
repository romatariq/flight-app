namespace App.Public.DTO.v1;

/// <summary>
/// User notification.
/// </summary>
public class UserNotification
{
    /// <summary>
    /// Identification.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Notification type identification.
    /// </summary>
    public Guid NotificationId { get; set; }

    /// <summary>
    /// Notification type name.
    /// </summary>
    public string NotificationType { get; set; } = default!;

    /// <summary>
    /// Minutes from notification (negative number means before).
    /// </summary>
    public int MinutesFromEvent { get; set; }
}