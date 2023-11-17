using App.Contracts.DAL.IRepositories;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;
using Base.Helpers;
using Dal = App.Private.DTO.DAL;

namespace App.DAL.EF.Repositories;

public class AirportRepository: EFBaseRepository<Domain.Airport, AppDbContext>, IAirportRepository
{
    public AirportRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Dal.Airport?> GetByIata(string iata)
    {
        return await DbSet
            .Include(a => a.Country)
            .Select(a => new Dal.Airport
            {
                Id = a.Id,
                Name = a.Name,
                Iata = a.Iata,
                CountryName = a.Country!.Name,
                CountryIso2 = a.Country.Iso2,
                CountryIso3 = a.Country.Iso3,
                DisplayFlights = a.DisplayAirport,
                DeparturesLastCheckedUtc = a.DeparturesLastCheckedUtc,
                ArrivalsLastCheckedUtc = a.ArrivalsLastCheckedUtc
            })
            .FirstOrDefaultAsync(a =>  a.Iata == iata.ToUpper());
    }

    public async Task<Dal.Airport?> GetRestDtoByIata(string iata)
    {
        return await DbSet
            .Include(a => a.Country)
            .Select(a => new Dal.Airport
            {
                Id = a.Id,
                Name = a.Name,
                Iata = a.Iata,
                CountryName = a.Country!.Name,
                CountryIso2 = a.Country.Iso2,
                CountryIso3 = a.Country.Iso3,
                DisplayFlights = a.DisplayAirport
            })
            .FirstOrDefaultAsync(a =>  a.Iata.ToLower() == iata.ToLower());
    }

    public IEnumerable<Dal.Airport> GetAll(string? filter)
    {
        const int maxResults = 25;
        filter = filter?.ToUpper();

        var query = DbSet
            .OrderBy(a => a.Name)
            .Include(a => a.Country)
            .Select(a => new Dal.Airport
            {
                Id = a.Id,
                Name = a.Name,
                Iata = a.Iata,
                CountryName = a.Country!.Name,
                CountryIso2 = a.Country.Iso2,
                CountryIso3 = a.Country.Iso3,
                DisplayFlights = a.DisplayAirport
            })
            .AsQueryable();
        
        if (filter is null or "")
        {
            return query.Take(maxResults);
        }
        
        var finalQuery = query
            .Where(a => 
                a.Iata == filter ||
                a.CountryIso2 == filter ||
                a.CountryIso3 == filter);
        
        finalQuery = finalQuery.Union(
            query.Where(a => 
                    a.Name.ToUpper().StartsWith(filter))
                .Take(Math.Max(maxResults - finalQuery.Count(), 0))
        );
        if (finalQuery.Count() >= maxResults) return finalQuery;
        
        finalQuery = finalQuery.Union(
            query.Where(a => 
                    a.CountryName.ToUpper().StartsWith(filter))
                .Take(Math.Max(maxResults - finalQuery.Count(), 0))
        );
        if (finalQuery.Count() >= maxResults) return finalQuery;

        finalQuery = finalQuery.Any() ? finalQuery : finalQuery.Union(
            query.Where(a => 
                    a.Name.ToUpper().Contains(filter))
                .Take(Math.Max(maxResults - finalQuery.Count(), 0))
        );
        if (finalQuery.Count() >= maxResults) return finalQuery;

        finalQuery = finalQuery.Any() ? finalQuery : finalQuery.Union(
            query.Where(a => 
                    a.CountryName.ToUpper().Contains(filter))
                .Take(Math.Max(maxResults - finalQuery.Count(), 0))
        );        
        
        return finalQuery;
    }

    public async Task<Dal.AirportStatistics?> GetStatistics(string airportIata, int timePeriodHours = 24)
    {
        var dateNow = DateTime.UtcNow.RemoveKind();
        var dateFrom = dateNow.AddHours(-timePeriodHours);
        return await DbSet
            .Include(a => a.DepartureFlights!)
                .ThenInclude(f => f.Airline)
            .Include(a => a.DepartureFlights!)
                .ThenInclude(f => f.ArrivalAirport!)
                    .ThenInclude(a => a.Country)
            
            .Include(a => a.ArrivalFlights!)
                .ThenInclude(f => f.Airline)
            .Include(a => a.ArrivalFlights!)
                .ThenInclude(f => f.DepartureAirport!)
                    .ThenInclude(a => a.Country)
            .Where(a => a.Iata == airportIata.ToUpper())
            
            .Select(a => new Dal.AirportStatisticsTemporary
            {
                AirportName = a.Name,
                AirportIata = a.Iata,
                Departures = a.DepartureFlights!.Where(f => timePeriodHours == 0 || f.ScheduledDepartureUtc >= dateFrom && f.ScheduledDepartureUtc <= dateNow),
                Arrivals = a.ArrivalFlights!.Where(f => timePeriodHours == 0 || f.ScheduledArrivalUtc >= dateFrom && f.ScheduledArrivalUtc <= dateNow),
            })
            .Select(a => new Dal.AirportStatistics
            {
                AirportName = a.AirportName,
                AirportIata = a.AirportIata,
                DeparturesCount = a.Departures.Count(),
                ArrivalsCount = a.Arrivals.Count(),
                DepartureAirlines = a.Departures
                    .Select(f => f.Airline!.Name)
                    .Distinct()
                    .Select(airline => new Dal.NameCounter
                    {
                        Name = airline,
                        Count = a.Departures.Count(f => f.Airline!.Name == airline)
                    })
                    .OrderByDescending(x => x.Count)
                    .Take(6),
                ArrivalAirlines = a.Arrivals
                    .Select(f => f.Airline!.Name)
                    .Distinct()
                    .Select(airline => new Dal.NameCounter
                    {
                        Name = airline,
                        Count = a.Arrivals.Count(f => f.Airline!.Name == airline)
                    })
                    .OrderByDescending(x => x.Count)
                    .Take(6),
                DepartureCountries = a.Departures
                    .Select(f => f.ArrivalAirport!.Country!.Name)
                    .Distinct()
                    .Select(country => new Dal.NameCounter
                    {
                        Name = country,
                        Count = a.Departures.Count(f => f.ArrivalAirport!.Country!.Name == country)
                    })
                    .OrderByDescending(x => x.Count)
                    .Take(6),
                ArrivalCountries = a.Arrivals
                    .Select(f => f.DepartureAirport!.Country!.Name)
                    .Distinct()
                    .Select(country => new Dal.NameCounter
                    {
                        Name = country,
                        Count = a.Arrivals.Count(f => f.DepartureAirport!.Country!.Name == country)
                    })
                    .OrderByDescending(x => x.Count)
                    .Take(6)
            })
            .SingleOrDefaultAsync();
    }

    public async Task SetLastCheckedUtc(Guid airportId, DateTime dateTime, bool isDeparture)
    {
        var airport = await DbSet.FindAsync(airportId);
        if (airport == null) return;
     
        if (isDeparture)
        {
            airport.DeparturesLastCheckedUtc = dateTime;
        } else
        {
            airport.ArrivalsLastCheckedUtc = dateTime;
        }
    }
}