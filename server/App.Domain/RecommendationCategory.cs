using System.ComponentModel.DataAnnotations;
using Base.Domain;

namespace App.Domain;

public class RecommendationCategory: DomainEntityId
{

    [MaxLength(100)]
    public string Category { get; set; } = default!;


    public ICollection<Recommendation>? Recommendations { get; set; }
}