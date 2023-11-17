using Base.Domain;

namespace App.Private.DTO.DAL;

public class RecommendationCategory: DomainEntityId
{
    public string Category { get; set; } = default!;
}