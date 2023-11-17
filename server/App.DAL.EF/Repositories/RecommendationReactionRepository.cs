using App.Contracts.DAL.IRepositories;
using Base.DAL.EF;
using App.Domain;
using App.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Dal = App.Private.DTO.DAL;

namespace App.DAL.EF.Repositories;

public class RecommendationReactionRepository: EFBaseRepository<Domain.RecommendationReaction, AppDbContext>, IRecommendationReactionRepository
{
    public RecommendationReactionRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Dal.RecommendationReaction?> Add(Guid reviewId, AppUser user, int userFeedback)
    {
        if (userFeedback is not (1 or -1)) return null;
        
        var review = await DbContext.Recommendations
            .SingleOrDefaultAsync(r => r.Id == reviewId);
        if (review == null)
        {
            return null;
        }

        var reaction = DbSet
            .SingleOrDefault(r => 
                r.AppUserId == user.Id && 
                r.RecommendationId == review.Id);
        if (reaction != null) return null;
        
        reaction = new Domain.RecommendationReaction()
        {
            AppUserId = user.Id,
            RecommendationId = review.Id,
            IsPositiveReaction = userFeedback == 1
        };
        var res = (await DbSet.AddAsync(reaction)).Entity;
        return new Dal.RecommendationReaction
        {
            Id = res.Id,
            AppUserId = res.AppUserId,
            IsPositiveReaction = res.IsPositiveReaction,
            RecommendationId = res.RecommendationId,
            CreatedAtUtc = res.CreatedAtUtc
        };
    }

    public Dal.RecommendationReaction? Update(Guid reviewId, AppUser user, int userFeedback)
    {
        if (userFeedback is not (1 or -1)) return null;

        var reaction = DbSet
            .SingleOrDefault(r => 
                r.AppUserId == user.Id &&
                r.RecommendationId == reviewId);
        
        var isUserFeedbackPositive = userFeedback == 1;

        if (reaction == null || 
            reaction.IsPositiveReaction == isUserFeedbackPositive) return null;
        
        reaction.IsPositiveReaction = isUserFeedbackPositive;
        var res = DbSet.Update(reaction).Entity;
        return new Dal.RecommendationReaction
        {
            Id = res.Id,
            AppUserId = res.AppUserId,
            IsPositiveReaction = res.IsPositiveReaction,
            RecommendationId = res.RecommendationId,
            CreatedAtUtc = res.CreatedAtUtc
        };
    }

    public bool Delete(Guid reviewId, AppUser user)
    {
        var reaction = DbSet
            .SingleOrDefault(r => 
                r.AppUserId == user.Id &&
                r.RecommendationId == reviewId);

        var res = reaction == null ? 
            null : 
            DbSet.Remove(reaction).Entity;
        return res != null;
    }
}