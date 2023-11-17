namespace App.Public.DTO.v1;

/// <summary>
/// Recommendation/review info.
/// </summary>
public class Recommendation
{
    /// <summary>
    /// Optional identification (null when creating).
    /// </summary>
    public Guid? Id { get; set; }
    
    /// <summary>
    /// Category of recommendation/review.
    /// </summary>
    public RecommendationCategory Category { get; set; } = default!;

    /// <summary>
    /// Iata (3 letter code) of the airport recommendation is about.
    /// </summary>
    public string AirportIata { get; set; } = default!;

    /// <summary>
    /// Recommendation author full name.
    /// </summary>
    public string AuthorName { get; set; } = default!;
    
    /// <summary>
    /// Identification of recommendation author.
    /// </summary>
    public Guid AuthorId { get; set; }
    
    /// <summary>
    /// Recommendation text.
    /// </summary>
    public string Text { get; set; } = default!;
    
    /// <summary>
    /// Recommendation creation date and time in UTC.
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Rating of recommendation/review (1-5).
    /// </summary>
    public decimal Rating { get; set; }

    /// <summary>
    /// User reaction to recommendation (1 - positive; -1 - negative; 0 none).
    /// </summary>
    public int UserFeedback { get; set; }

    /// <summary>
    /// Sum of all reactions.
    /// </summary>
    public int UsersFeedback { get; set; }
}