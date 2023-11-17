using Base.Contracts.DAL;
using App.Domain.Identity;
using Dal = App.Private.DTO.DAL;


namespace App.Contracts.DAL.IRepositories;

public interface IUserFlightRepository: IBaseRepository<Dal.UserFlightInfo>, IUserFlightRepositoryCustom<Dal.UserFlightInfo>
{
    Task<List<Dal.UserFlightStatistics>> GetStatisticsAsync(Guid userId);
}


public interface IUserFlightRepositoryCustom<TEntity>
{
    public Task<Guid> AddAsync(AppUser appUser, Guid flightId);
    
    public Task<IEnumerable<TEntity>> GetAllAsync(AppUser appUser);
    
    public Task<bool> DeleteAsync(Guid id, AppUser appUser);
}