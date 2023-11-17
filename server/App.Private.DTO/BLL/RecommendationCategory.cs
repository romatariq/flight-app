using Base.Domain;

namespace App.Private.DTO.BLL;

public class RecommendationCategory: DomainEntityId
{
    public string Category { get; set; } = default!;
}