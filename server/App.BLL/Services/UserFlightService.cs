using App.Contracts.BLL.IServices;
using App.Contracts.DAL;
using App.Contracts.DAL.IRepositories;
using App.Domain.Identity;
using Base.BLL;
using Bll = App.Private.DTO.BLL;
using Dal = App.Private.DTO.DAL;
using DomainDto = App.Domain;
using AutoMappers = App.Mappers.AutoMappers;

namespace App.BLL.Services;

public class UserFlightService :
    BaseEntityService<Dal.UserFlightInfo, Bll.UserFlightInfo, IUserFlightRepository>, IUserFlightService
{
    protected IAppUOW Uow;


    public UserFlightService(IAppUOW uow, AutoMapper.IMapper mapper)
        : base(uow.UserFlightRepository, new AutoMappers.BLL.UserFlightMapper(mapper))
    {
        Uow = uow;
    }

    public async Task<Guid> AddAsync(AppUser appUser, Guid flightId)
    {
        return await Uow.UserFlightRepository.AddAsync(appUser, flightId);
    }

    public async Task<IEnumerable<Bll.UserFlightInfo>> GetAllAsync(AppUser appUser)
    {
        return (await Uow.UserFlightRepository.GetAllAsync(appUser))
            .Select(Mapper.Map)!;
    }

    public async Task<bool> DeleteAsync(Guid id, AppUser appUser)
    {
        return await Uow.UserFlightRepository.DeleteAsync(id, appUser);
    }

    public async Task<Bll.UserFlightsStatistics> GetStatisticsAsync(Guid userId)
    {
        var flights = await Uow.UserFlightRepository.GetStatisticsAsync(userId);
        return new Bll.UserFlightsStatistics
        {
            Count = flights.Count,
            TotalDistance = (int) flights
                .Sum(f => 
                    Base.Helpers.DistanceHelpers.GetDistanceInMeters(
                        f.DepartureAirportLatitude, 
                        f.DepartureAirportLongitude, 
                        f.ArrivalAirportLatitude, 
                        f.ArrivalAirportLongitude) / 1000),
            TotalTimeMinutes = (int) flights
                .Sum(f => 
                    (f.ScheduledArrivalUtc - f.ScheduledDepartureUtc).TotalMinutes),
            TotalTimeDelayedDepartureMinutes = (int) flights
                .Sum(f => 
                    (f.ExpectedDepartureUtc - f.ScheduledDepartureUtc).TotalMinutes),
            TotalTimeDelayedArrivalMinutes = (int) flights
                .Sum(f =>
                    (f.ExpectedArrivalUtc - f.ScheduledArrivalUtc).TotalMinutes)
        };
    }
}
