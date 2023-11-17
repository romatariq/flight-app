using Base.Contracts.DAL;
using App.Domain.Identity;
using Dal = App.Private.DTO.DAL;

namespace App.Contracts.DAL.IRepositories;

public interface IRecommendationReactionRepository: IBaseRepository<Dal.RecommendationReaction>, 
    IRecommendationReactionRepositoryCustom<Dal.RecommendationReaction>
{
}


public interface IRecommendationReactionRepositoryCustom<TEntity>
{
    public Task<TEntity?> Add(Guid reviewId, AppUser user, int userFeedback);
    
    public TEntity? Update(Guid reviewId, AppUser user, int userFeedback);
    
    public bool Delete(Guid reviewId, AppUser user);
}