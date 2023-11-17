using App.Contracts.BLL.IServices;
using App.Contracts.DAL;
using App.Contracts.DAL.IRepositories;
using Base.BLL;
using Base.Contracts;
using Bll = App.Private.DTO.BLL;
using Dal = App.Private.DTO.DAL;
using DomainDto = App.Domain;
using AutoMappers = App.Mappers.AutoMappers;

namespace App.BLL.Services;

public class AirportService :
    BaseEntityService<Dal.Airport, Bll.Airport, IAirportRepository>, IAirportService
{
    protected IAppUOW Uow;
    protected readonly IMapper<Dal.AirportStatistics, Bll.AirportStatistics> StatsMapper;

    public AirportService(IAppUOW uow, AutoMapper.IMapper mapper)
        : base(uow.AirportRepository, new AutoMappers.BLL.AirportMapper(mapper))
    {
        Uow = uow;
        StatsMapper = new AutoMappers.BLL.AirportStatisticsMapper(mapper);
    }


    public async Task<Bll.Airport?> GetByIata(string iata)
    {
        return Mapper.Map(
            await Uow.AirportRepository.GetByIata(iata)
        );
    }

    public async Task<Bll.Airport?> GetRestDtoByIata(string iata)
    {
        return Mapper.Map(
            await Uow.AirportRepository.GetRestDtoByIata(iata)
        );
    }

    public IEnumerable<Bll.Airport> GetAll(string? filter)
    {
        return Uow.AirportRepository.GetAll(filter)
            .Select(Mapper.Map)!;
    }

    public async Task<Bll.AirportStatistics?> GetStatistics(string airportIata, int timePeriodHours = 24)
    {
        var stats = await Uow.AirportRepository.GetStatistics(airportIata, timePeriodHours);
        if (stats == null) return null;
        
        var mappedAirportStats = StatsMapper.Map(stats);

        if (mappedAirportStats!.DepartureCountries.Any())
        {
            var list = mappedAirportStats.DepartureCountries.ToList();
            var unknown = list
                .FirstOrDefault(x => x.Name.ToLower() == "unknown");
            if (unknown != null)
            {
                list.Remove(unknown);
            }

            list = list
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToList();
            
            var count = list.Select(x => x.Count).Sum();
            list.Add(new Bll.NameCounter()
            {
                Name = "Other",
                Count = mappedAirportStats.DeparturesCount - count - (unknown?.Count ?? 0)
            });
            mappedAirportStats.DepartureCountries = list;
        }
        if (mappedAirportStats.ArrivalCountries.Any())
        {
            var list = mappedAirportStats.ArrivalCountries.ToList();
            var unknown = list
                .FirstOrDefault(x => x.Name.ToLower() == "unknown");
            if (unknown != null)
            {
                list.Remove(unknown);
            }
            
            list = list
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToList();
            
            var count = list.Select(x => x.Count).Sum();
            list.Add(new Bll.NameCounter()
            {
                Name = "Other",
                Count = mappedAirportStats.ArrivalsCount - count - (unknown?.Count ?? 0)
            });
            mappedAirportStats.ArrivalCountries = list;
        }
        if (mappedAirportStats.DepartureAirlines.Any())
        {
            var list = mappedAirportStats.DepartureAirlines.ToList();
            var unknown = list
                .FirstOrDefault(x => x.Name.ToLower() == "unknown");
            if (unknown != null)
            {
                list.Remove(unknown);
            }
            
            list = list
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToList();
            
            var count = list.Select(x => x.Count).Sum();
            list.Add(new Bll.NameCounter()
            {
                Name = "Other",
                Count = mappedAirportStats.DeparturesCount - count - (unknown?.Count ?? 0)
            });
            mappedAirportStats.DepartureAirlines = list;
        }        
        if (mappedAirportStats.ArrivalAirlines.Any())
        {
            var list = mappedAirportStats.ArrivalAirlines.ToList();
            var unknown = list
                .FirstOrDefault(x => x.Name.ToLower() == "unknown");
            if (unknown != null)
            {
                list.Remove(unknown);
            }
            
            list = list
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToList();
            
            var count = list.Select(x => x.Count).Sum();
            list.Add(new Bll.NameCounter()
            {
                Name = "Other",
                Count = mappedAirportStats.ArrivalsCount - count - (unknown?.Count ?? 0)
            });
            mappedAirportStats.ArrivalAirlines = list;
        }

        return mappedAirportStats;
    }
}
