using Base.Contracts.DAL;
using App.Private.DTO.DataCollector;
using FlightInfoCollector;
using Dal = App.Private.DTO.DAL;

namespace App.Contracts.DAL.IRepositories;

public interface IFlightRepository: IBaseRepository<Dal.FlightInfo>, IFlightRepositoryCustom<Dal.FlightInfo, Dal.FlightInfoDetails>
{
    Task AddOrUpdateAsync(IEnumerable<FlightData> flights);

    Task FetchNewestFlightAndUpdateIfNecessary(Guid id, IDataCollector dataCollector);
}



public interface IFlightRepositoryCustom<TEntity, TEntityDetails>
{
    Task<IEnumerable<TEntity>> GetDepartures(string airportIata);
    
    Task<IEnumerable<TEntity>> GetArrivals(string airportIata);

    Task<TEntityDetails?> GetFlightAsync(Guid flightId, Guid? appUserId);
}