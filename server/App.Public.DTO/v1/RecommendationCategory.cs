namespace App.Public.DTO.v1;

/// <summary>
/// Recommendation/review category.
/// </summary>
public class RecommendationCategory
{
    /// <summary>
    /// Identification.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Category name.
    /// </summary>
    public string Category { get; set; } = default!;
}