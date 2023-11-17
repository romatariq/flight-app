using App.Domain.Identity;
using Base.Domain;

namespace App.Domain;

public class RecommendationReaction: DomainEntityId
{

    public bool IsPositiveReaction { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    
    public Guid RecommendationId { get; set; }
    public Recommendation? Recommendation { get; set; }

    public Guid AppUserId { get; set; }
    public AppUser? AppUser { get; set; }
}