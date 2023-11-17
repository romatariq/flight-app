namespace App.Public.DTO.v1;

/// <summary>
/// Notification type.
/// </summary>
public class Notification
{
    /// <summary>
    /// Identification.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Notification type name.
    /// </summary>
    public string Type { get; set; } = default!;
}