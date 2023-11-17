namespace App.Public.DTO.v1;

/// <summary>
/// Required info to create user flight notification.
/// </summary>
public class MinimalUserFlightNotification
{
    /// <summary>
    /// Identification of user flight.
    /// </summary>
    public Guid UserFlightId { get; set; }

    /// <summary>
    /// Minutes from event to send notification.
    /// </summary>
    public int MinutesFromEvent { get; set; }

    /// <summary>
    /// Notification type Identification.
    /// </summary>
    public Guid NotificationId { get; set; }
}