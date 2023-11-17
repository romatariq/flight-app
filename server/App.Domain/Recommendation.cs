using System.ComponentModel.DataAnnotations;
using App.Domain.Identity;
using Base.Domain;

namespace App.Domain;

public class Recommendation: DomainEntityId
{

    [MaxLength(1000)]
    public string RecommendationText { get; set; } = default!;

    public decimal Rating { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    

    public Guid AirportId { get; set; }
    public Airport? Airport { get; set; }

    public Guid RecommendationCategoryId { get; set; }
    public RecommendationCategory? RecommendationCategory { get; set; }

    public Guid AppUserId { get; set; }
    public AppUser? AppUser { get; set; }

    public ICollection<RecommendationReaction>? RecommendationReactions { get; set; }
}