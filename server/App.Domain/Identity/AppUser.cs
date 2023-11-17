using System.ComponentModel.DataAnnotations;
using Base.Contracts.Domain;
using Microsoft.AspNetCore.Identity;

namespace App.Domain.Identity;

public class AppUser: IdentityUser<Guid>, IDomainEntityId
{
    [MaxLength(128)]
    public string FirstName { get; set; } = default!;
    
    [MaxLength(128)]
    public string LastName { get; set; } = default!;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public bool IsVerified { get; set; }
    
    
    public ICollection<Recommendation>? Recommendations { get; set; }

    public ICollection<RecommendationReaction>? RecommendationReactions { get; set; }
    
    public ICollection<UserFlight>? UserFlights { get; set; }
    
    public ICollection<AppRefreshToken>? AppRefreshTokens { get; set; }
}