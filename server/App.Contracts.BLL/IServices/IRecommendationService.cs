using App.Contracts.DAL.IRepositories;
using Base.Contracts.DAL;
using Bll = App.Private.DTO.BLL;

namespace App.Contracts.BLL.IServices;

public interface IRecommendationService : IBaseRepository<Bll.Recommendation>, 
    IRecommendationRepositoryCustom<Bll.Recommendation>
{
    // add your custom service methods here
}