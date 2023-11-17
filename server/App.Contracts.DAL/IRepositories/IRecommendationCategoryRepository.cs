using Base.Contracts.DAL;
using Dal = App.Private.DTO.DAL;

namespace App.Contracts.DAL.IRepositories;

public interface IRecommendationCategoryRepository: IBaseRepository<Dal.RecommendationCategory>, IRecommendationCategoryRepositoryCustom<Dal.RecommendationCategory>
{
    
}


public interface IRecommendationCategoryRepositoryCustom<TEntity>
{
    Task<IEnumerable<TEntity>> GetAllAsync();
}