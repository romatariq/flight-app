using App.Contracts.DAL.IRepositories;
using Base.DAL.EF;
using App.Domain;
using App.Domain.Identity;
using App.Private.DTO.DAL;
using Microsoft.EntityFrameworkCore;
using Dal = App.Private.DTO.DAL;

namespace App.DAL.EF.Repositories;

public class UserFlightRepository: EFBaseRepository<UserFlight, AppDbContext>, IUserFlightRepository
{
    public UserFlightRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Guid> AddAsync(AppUser appUser, Guid flightId)
    {
        var flight = await DbContext.Flights.FindAsync(flightId);
        if (flight == null)
        {
            throw new Exception("Flight not found");
        }
        
        var existing = await DbSet
            .Where(uf => 
                uf.AppUserId == appUser.Id && 
                uf.FlightId == flight.Id)
            .SingleOrDefaultAsync();
        if (existing != null) return existing.Id;
        
        var userFlight = new UserFlight()
        {
            AppUserId = appUser.Id,
            FlightId = flight.Id,
        };
        var res = (await DbSet.AddAsync(userFlight)).Entity;
        return res.Id;
    }
    

    public async Task<IEnumerable<Dal.UserFlightInfo>> GetAllAsync(AppUser appUser)
    {
        return await DbSet
            .Include(uf => uf.Flight)
                .ThenInclude(f => f!.DepartureAirport)
            .Include(uf => uf.Flight)
                .ThenInclude(f => f!.ArrivalAirport)
            .Where(uf => uf.AppUserId == appUser.Id)
            .Select(uf => new UserFlightInfo
            {
                Id = uf.Id,
                FlightId = uf.FlightId,
                FlightIata = uf.Flight!.FlightIata,
                DepartureAirportIata = uf.Flight.DepartureAirport!.Iata,
                ArrivalAirportIata = uf.Flight.ArrivalAirport!.Iata,
                ScheduledDepartureUtc = uf.Flight.ScheduledDepartureUtc,
                ScheduledArrivalUtc = uf.Flight.ScheduledArrivalUtc,
                DepartureAirportLatitude = uf.Flight.DepartureAirport.Latitude,
                DepartureAirportLongitude = uf.Flight.DepartureAirport.Longitude,
                ArrivalAirportLatitude = uf.Flight.ArrivalAirport.Latitude,
                ArrivalAirportLongitude = uf.Flight.ArrivalAirport.Longitude,
            })
            .ToListAsync();
    }

    public async Task<bool> DeleteAsync(Guid userFlightId, AppUser appUser)
    {
        var userFlight = await DbSet
            .Where(uf => 
                uf.Id == userFlightId && uf.AppUserId == appUser.Id)
            .SingleOrDefaultAsync();
        if (userFlight == null) return false;
        
        var notifications = await DbContext.UserFlightNotifications
            .Where(ufn => ufn.UserFlightId == userFlight.Id)
            .ToListAsync();
        DbContext.UserFlightNotifications.RemoveRange(notifications);
        DbSet.Remove(userFlight);
        return true;
    }

    public async Task<List<UserFlightStatistics>> GetStatisticsAsync(Guid userId)
    {
        return await DbSet
            .Include(uf => uf.Flight)
            .ThenInclude(f => f!.DepartureAirport)
            .Include(uf => uf.Flight)
            .ThenInclude(f => f!.ArrivalAirport)
            .Where(uf => uf.AppUserId == userId)
            .Select(uf => new Dal.UserFlightStatistics
            {
                DepartureAirportLatitude = uf.Flight!.DepartureAirport!.Latitude,
                DepartureAirportLongitude = uf.Flight.DepartureAirport.Longitude,
                ArrivalAirportLatitude = uf.Flight.ArrivalAirport!.Latitude,
                ArrivalAirportLongitude = uf.Flight.ArrivalAirport.Longitude,
                ScheduledDepartureUtc = uf.Flight.ScheduledDepartureUtc,
                ScheduledArrivalUtc = uf.Flight.ScheduledArrivalUtc,
                ExpectedDepartureUtc = uf.Flight.ExpectedDepartureUtc,
                ExpectedArrivalUtc = uf.Flight.ExpectedArrivalUtc
            })
            .ToListAsync();
    }
}