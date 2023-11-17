using App.Contracts.DAL.IRepositories;
using Base.Contracts.DAL;
using Bll = App.Private.DTO.BLL;

namespace App.Contracts.BLL.IServices;

public interface IRecommendationReactionService : IBaseRepository<Bll.RecommendationReaction>, 
    IRecommendationReactionRepositoryCustom<Bll.RecommendationReaction>
{
    // add your custom service methods here
}