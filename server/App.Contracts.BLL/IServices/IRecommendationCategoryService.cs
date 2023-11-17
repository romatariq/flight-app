using App.Contracts.DAL.IRepositories;
using Base.Contracts.DAL;

namespace App.Contracts.BLL.IServices;
using Bll = App.Private.DTO.BLL;

public interface IRecommendationCategoryService: IBaseRepository<Bll.RecommendationCategory>, 
    IRecommendationCategoryRepositoryCustom<Bll.RecommendationCategory>
{
}