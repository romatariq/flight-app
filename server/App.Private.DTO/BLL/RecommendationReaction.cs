using Base.Domain;

namespace App.Private.DTO.BLL;

public class RecommendationReaction: DomainEntityId
{
    public bool IsPositiveReaction { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    
    public Guid RecommendationId { get; set; }

    public Guid AppUserId { get; set; }
}