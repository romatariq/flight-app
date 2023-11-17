using Base.Contracts.DAL;
using Dal = App.Private.DTO.DAL;

namespace App.Contracts.DAL.IRepositories;

public interface IAirportRepository: IBaseRepository<Dal.Airport>, IAirportRepositoryCustom<Dal.Airport, Dal.AirportStatistics>
{
    // custom methods for only repository
    Task SetLastCheckedUtc(Guid airportId, DateTime dateTime, bool isDeparture);
}

public interface IAirportRepositoryCustom<TEntity, TEntity2>
{
    // custom methods shared between repository and service
    Task<TEntity?> GetByIata(string iata);
    
    
    Task<TEntity?> GetRestDtoByIata(string iata);
    
    IEnumerable<TEntity> GetAll(string? filter);
    
    Task<TEntity2?> GetStatistics(string airportIata, int timePeriodHours = 24);
}