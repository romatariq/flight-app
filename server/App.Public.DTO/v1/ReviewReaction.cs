namespace App.Public.DTO.v1;

/// <summary>
/// Review/recommendation reaction.
/// </summary>
public class ReviewReaction
{
    /// <summary>
    /// Review identification.
    /// </summary>
    public Guid ReviewId { get; set; }

    /// <summary>
    /// Reaction type as integer (1 - positive, -1 - negative, 0 - none).
    /// </summary>
    public int Feedback { get; set; }
}