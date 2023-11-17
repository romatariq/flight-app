using Base.Contracts.DAL;
using App.Domain.Identity;
using Dal = App.Private.DTO.DAL;

namespace App.Contracts.DAL.IRepositories;

public interface IRecommendationRepository: IBaseRepository<Dal.Recommendation>, IRecommendationRepositoryCustom<Dal.Recommendation>
{
}

public interface IRecommendationRepositoryCustom<TEntity>
{
    public Task<IEnumerable<TEntity>> 
        GetAllAsync(string airportIata, Guid categoryId, int page = 1, int pageSize = 10, AppUser? user = null);
    
    public int GetAllPageCount(string airportIata, Guid categoryId, int pageSize = 10);
    
    public Task<TEntity?> GetRestDtoAsync(Guid id);
    
    public Task<Guid?> AddAsync(Guid categoryId, string airportIata, decimal rating, string text, AppUser author);
    
    public Task<bool> UpdateAsync(Guid id, Guid categoryId, decimal rating, string text, AppUser author);
    
    public Task<bool> DeleteAsync(Guid id, AppUser author);
}