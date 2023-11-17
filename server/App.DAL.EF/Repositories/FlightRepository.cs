using App.Contracts.DAL.IRepositories;
using Base.DAL.EF;
using App.Domain;
using App.Domain.Identity;
using App.Helpers;
using App.Private.DTO.DAL;
using App.Private.DTO.DataCollector;
using Base.Helpers;
using FlightInfoCollector;
using Microsoft.EntityFrameworkCore;
using Aircraft = App.Private.DTO.DAL.Aircraft;

namespace App.DAL.EF.Repositories;

public class FlightRepository: EFBaseRepository<Domain.Flight, AppDbContext>, IFlightRepository
{

    public FlightRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<FlightInfo>> GetDepartures(string airportIata)
    {
        return await GetDeparturesOrArrivals(airportIata, true);
    }
    
    public async Task<IEnumerable<FlightInfo>> GetArrivals(string airportIata)
    {
        return await GetDeparturesOrArrivals(airportIata, false);
    }

    private async Task<IEnumerable<FlightInfo>> GetDeparturesOrArrivals(string airportIata, bool isDeparture)
    {
        var airport = await DbContext.Airports
            .FirstAsync(a => a.Iata == airportIata.ToUpper());
        if (airport == null)
        {
            throw new Exception("Airport not found");
        }

        var dateNow = DateTime.UtcNow.RemoveKind();
        
        return DbSet
            .Include(f => f.FlightStatus)
            .Include(f => f.Airline)
            .Include(f => f.DepartureAirport)
            .Include(f => f.ArrivalAirport)
            .Where(f =>
                (isDeparture ? f.DepartureAirportId : f.ArrivalAirportId) == airport.Id &&
                (isDeparture ? f.ScheduledDepartureUtc : f.ScheduledArrivalUtc).AddHours(12) > dateNow &&
                (isDeparture ? f.ScheduledDepartureUtc : f.ScheduledArrivalUtc) < dateNow.AddHours(12))
            .Select(f => new FlightInfo
            {
                Id = f.Id,
                FlightIata = f.FlightIata,
                ScheduledDepartureUtc = f.ScheduledDepartureUtc,
                ScheduledArrivalUtc = f.ScheduledArrivalUtc,
                Airline = f.Airline!.Name,
                Status = f.FlightStatus!.Name,
                DepartureAirportIata = f.DepartureAirport!.Iata,
                ArrivalAirportIata = f.ArrivalAirport!.Iata,
                DepartureAirportName = f.DepartureAirport.Name,
                ArrivalAirportName = f.ArrivalAirport.Name,
                DepartureAirportLatitude = f.DepartureAirport.Latitude,
                DepartureAirportLongitude = f.DepartureAirport.Longitude,
                ArrivalAirportLatitude = f.ArrivalAirport.Latitude,
                ArrivalAirportLongitude = f.ArrivalAirport.Longitude
            });
    } 
    

    public async Task<FlightInfoDetails?> GetFlightAsync(Guid flightId, Guid? appUserId)
    {
        return await DbSet
            .Include(f => f.Aircraft)
                .ThenInclude(a => a == null ? null : a.AircraftModel)
            .Include(f => f.FlightStatus)
            .Include(f => f.ArrivalAirport)
            .Include(f => f.DepartureAirport)
            .Include(f => f.Airline)
            .Include(f => f.UserFlights)
            .Select(f => new FlightInfoDetails
            {
                Id = f.Id,
                FlightIata = f.FlightIata,
                ScheduledDepartureUtc = f.ScheduledDepartureUtc,
                ScheduledArrivalUtc = f.ScheduledArrivalUtc,
                EstimatedDepartureUtc = f.ExpectedDepartureUtc,
                EstimatedArrivalUtc = f.ExpectedArrivalUtc,
                Airline = f.Airline!.Name,
                Status = f.FlightStatus!.Name,
                DepartureAirportIata = f.DepartureAirport!.Iata,
                ArrivalAirportIata = f.ArrivalAirport!.Iata,
                DepartureAirportName = f.DepartureAirport.Name,
                ArrivalAirportName = f.ArrivalAirport.Name,
                DepartureAirportLatitude = f.DepartureAirport.Latitude,
                DepartureAirportLongitude = f.DepartureAirport.Longitude,
                ArrivalAirportLatitude = f.ArrivalAirport.Latitude,
                ArrivalAirportLongitude = f.ArrivalAirport.Longitude,
                DisplayDepartureTerminal = f.DepartureAirport.DisplayTerminal,
                DisplayArrivalTerminal = f.ArrivalAirport.DisplayTerminal,
                DisplayDepartureGate = f.DepartureAirport.DisplayGate,
                DisplayArrivalGate = f.ArrivalAirport.DisplayGate,
                DepartureTerminal = f.DepartureTerminal,
                ArrivalTerminal = f.ArrivalTerminal,
                DepartureGate = f.DepartureGate,
                ArrivalGate = f.ArrivalGate,
                Aircraft = f.Aircraft == null
                    ? null
                    : new Aircraft
                    {
                        Id = f.Aircraft.Id,
                        Icao = f.Aircraft.IcaoHex,
                        Registration = f.Aircraft.RegistrationNumber,
                        Longitude = f.Aircraft.Longitude,
                        Latitude = f.Aircraft.Latitude,
                        SpeedKmh = f.Aircraft.SpeedKmh,
                        ModelName = f.Aircraft.AircraftModel!.ModelName,
                        InfoLastUpdatedUtc = f.Aircraft.InfoLastUpdatedUtc
                    },
                UserFlightId = f.UserFlights!
                    .FirstOrDefault(uf => uf.AppUserId == appUserId) == null ? 
                        null : 
                        f.UserFlights!.First(uf => uf.AppUserId == appUserId).Id
            })
            .FirstOrDefaultAsync(f => 
                f.Id == flightId);
    }

    public async Task FetchNewestFlightAndUpdateIfNecessary(Guid id, IDataCollector dataCollector)
    {
        var flight = await DbSet
            .Include(f => f.Aircraft)
                .ThenInclude(a => a == null ? null : a.AircraftModel)
            .Include(f => f.FlightStatus)
            .Include(f => f.ArrivalAirport)
            .Include(f => f.DepartureAirport)
            .Include(f => f.Airline)
            .FirstOrDefaultAsync(f => f.Id == id);
        if (flight == null) return;
        
        var dateNow = DateTime.UtcNow.RemoveKind();
        if (flight.FlightStatus!.Name == "Finished")
        {
            return;
        }
        if (flight.ExpectedArrivalUtc.AddHours(2) < dateNow)
        {
            flight.FlightStatus = DbContext.FlightStatuses.FirstAsync(s => s.Name == "Finished").Result;
            return;
        }
        if (flight.FlightStatus.Name == "Scheduled" && flight.ExpectedDepartureUtc.AddMinutes(30) < dateNow)
        {
            flight.FlightStatus = DbContext.FlightStatuses.FirstAsync(s => s.Name == "Live").Result;
        }

        if (flight.Aircraft == null && 
            (flight.FlightInfoLastCheckedUtc == null || flight.FlightInfoLastCheckedUtc.Value.AddMinutes(60) < dateNow))
        {
            var flightData = await dataCollector.GetFlightTechnicalInfo(flight);
            flight.FlightInfoLastCheckedUtc = dateNow;
            if (flightData != null)
            {
                var updatedFlight = await UpdateFlightDetails(flight, flightData);
                if (updatedFlight != null)
                {
                    flight = updatedFlight;
                }
            }
        }

        if (flight.Aircraft != null && flight.Aircraft.InfoLastUpdatedUtc.AddMinutes(15) < dateNow)
        {
            var flightLiveInfo = await dataCollector.GetAircraftLiveInfo(flight.Aircraft.IcaoHex);
            flight = UpdateFlightLiveInfo(flight, flightLiveInfo);
            
            if (flight.FlightStatus?.Name == "Live" && flight.Aircraft?.Latitude != null && flight.Aircraft?.Longitude != null
                && flight.DepartureAirport != null && flight.ArrivalAirport != null)
            {
                var flightTotalDistance = flight.DepartureAirport.GetDistanceInMeters(flight.ArrivalAirport);
                var flightFlownDistance = flight.DepartureAirport.GetDistanceInMeters(flight.Aircraft);
                var percentageFlown = flightFlownDistance / flightTotalDistance;
                var flightTime = (flight.ScheduledArrivalUtc - flight.ScheduledDepartureUtc).TotalMinutes;
                var timeFlown = flightTime * percentageFlown;
                flight.ExpectedArrivalUtc = DateTime.UtcNow.RemoveKind().AddMinutes(flightTime - timeFlown);
            }
            
            // get previous(current) flight estimated arrival
            else if (flight.Aircraft != null)
            {
                var thisFlightScheduledDeparture = flight.ScheduledDepartureUtc.RemoveKind();
                
                // Ignore compiler - it can be null
                DateTime? previousFlightEstimatedArrival = await DbSet
                    .Include(f => f.Aircraft)
                    .Where(f =>
                        f.Aircraft != null && f.Aircraft.IcaoHex == flight.Aircraft.IcaoHex &&
                        f.ScheduledDepartureUtc < thisFlightScheduledDeparture &&
                        f.ScheduledDepartureUtc > thisFlightScheduledDeparture.AddDays(-1))
                    .OrderByDescending(f => f.ScheduledDepartureUtc)
                    .Select(f => f.ExpectedArrivalUtc)
                    .FirstOrDefaultAsync();
                
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (previousFlightEstimatedArrival != null && previousFlightEstimatedArrival > thisFlightScheduledDeparture)
                {
                    var flightTime = (flight.ScheduledArrivalUtc - flight.ScheduledDepartureUtc).TotalMinutes;
                    flight.ExpectedDepartureUtc = previousFlightEstimatedArrival.Value.AddMinutes(15);
                    flight.ExpectedArrivalUtc = flight.ExpectedDepartureUtc.AddMinutes(flightTime);
                }
            }

        }
    }

    private async Task<Flight?> UpdateFlightDetails(Flight flight, FlightData flightData)
    {
        if (flight.Airline?.Name == "" && flightData.AirlineName != null)
        {
            flight.Airline.Name = flightData.AirlineName;
        }
        
        if (flightData.AircraftIcao == null) return null;
        
        flight.Aircraft = new Domain.Aircraft
        {
            IcaoHex = flightData.AircraftIcao,
            RegistrationNumber = flightData.AircraftRegistration ?? "",
            InfoLastUpdatedUtc = DateTime.MinValue,
            AircraftModel = await GetOrCreateAircraftModel(flightData)
        };

        return flight;
    }

    private Flight UpdateFlightLiveInfo(Flight flight, AircraftLiveData liveData)
    {
        if (flight.Aircraft == null) return flight;

        flight.Aircraft.Latitude = liveData.Latitude;
        flight.Aircraft.Longitude = liveData.Longitude;
        flight.Aircraft.SpeedKmh = liveData.Speed ?? 0;
        flight.Aircraft.InfoLastUpdatedUtc = DateTime.UtcNow;
        return flight;
    }


    public async Task AddOrUpdateAsync(IEnumerable<FlightData> flights)
    {
        foreach (var flight in flights)
        {
            if (flight.FlightIata == null || 
                flight.ScheduledDepartureUtc == null ||
                flight.ScheduledArrivalUtc == null || 
                flight.DepartureAirportIata == null ||
                flight.ArrivalAirportIata == null) continue;
            
            var flightFromDb = await DbContext.Flights
                .Include(f => f.FlightStatus)
                .Include(f => f.Aircraft)
                .Include(f => f.Airline)
                .FirstOrDefaultAsync(f => 
                    f.FlightIata == flight.FlightIata && 
                    f.ScheduledArrivalUtc == flight.ScheduledArrivalUtc && 
                    f.ScheduledDepartureUtc == flight.ScheduledDepartureUtc);
            
            if (flightFromDb != null)
            {
                // Update existing flight and possibly update connected entities
                await UpdateExistingFlight(flightFromDb, flight);
            }
            else
            {
                // Create new flight and possibly update connected entities
                try
                {
                    var newFlight = await GetFlightFromFlightData(flight);
                    await DbSet.AddAsync(newFlight);
                } catch (Exception)
                {
                    continue;
                }
            }
        }
    }
    
    private async Task<Flight> UpdateExistingFlight(Flight flightFromDb, FlightData flightData)
    {
        flightFromDb.DepartureTerminal = flightData.DepartureTerminal;
        flightFromDb.ArrivalTerminal = flightData.ArrivalTerminal;
        flightFromDb.DepartureGate = flightData.DepartureGate;
        flightFromDb.ArrivalGate = flightData.ArrivalGate;
        flightFromDb.FlightInfoLastCheckedUtc = DateTime.UtcNow;
        flightFromDb.ExpectedDepartureUtc = flightData.EstimatedDepartureUtc ?? flightFromDb.ExpectedDepartureUtc;
        flightFromDb.ExpectedArrivalUtc = flightData.EstimatedArrivalUtc ?? flightFromDb.ExpectedArrivalUtc;

        if (flightFromDb.Aircraft == null && flightData.AircraftIcao != null)
        {
            flightFromDb.Aircraft = await GetOrCreateAircraftFromFlightData(flightData);
        }

        if (flightFromDb.Airline!.Name == "")
        {
            flightFromDb.Airline = await GetOrCreateAirlineFromFlightData(flightData);
        }
        flightFromDb.FlightStatus = await GetOrCreateFlightStatusFromFlightData(flightData);
        return flightFromDb;
    }
    
    
    private async Task<Flight> GetFlightFromFlightData(FlightData flight)
    {
        if (flight.FlightIata == null || 
            flight.DepartureAirportIata == null || flight.ArrivalAirportIata == null  || 
            flight.ScheduledDepartureUtc == null || flight.ScheduledArrivalUtc == null)
        {
            throw new ArgumentException("Flight data is missing iata, airport iata or scheduled time");
        }
        
        return new Flight
        {
            FlightIata = flight.FlightIata,
            DepartureAirport = await DbContext.Airports.FirstAsync(a => 
                a.Iata == flight.DepartureAirportIata),
            ArrivalAirport = await DbContext.Airports.FirstAsync(a => 
                a.Iata == flight.ArrivalAirportIata),
            ScheduledDepartureUtc = flight.ScheduledDepartureUtc.Value,
            ScheduledArrivalUtc = flight.ScheduledArrivalUtc.Value,
            ExpectedDepartureUtc = flight.EstimatedDepartureUtc ?? flight.ScheduledDepartureUtc.Value,
            ExpectedArrivalUtc = flight.EstimatedArrivalUtc ?? flight.ScheduledArrivalUtc.Value,
            FlightStatus = await GetOrCreateFlightStatusFromFlightData(flight),
            Airline = await GetOrCreateAirlineFromFlightData(flight),
            Aircraft = flight.AircraftIcao == null ? 
                null : await GetOrCreateAircraftFromFlightData(flight)
        };
    }
    
    private async Task<Airline> GetOrCreateAirlineFromFlightData(FlightData flightData)
    {
        if (flightData.AirlineIata == null && flightData.AirlineIcao == null && flightData.AirlineName == null)
        {
            flightData.AirlineIata = "Unknown";
            flightData.AircraftIcao = "Unknown";
            flightData.AirlineName = "Unknown";
        }
        
        var airlineFromDb = await DbContext.Airlines.FirstOrDefaultAsync(a => 
            a.Iata == flightData.AirlineIata || 
            a.Icao == flightData.AirlineIcao || 
            a.Name == flightData.AirlineName);

        if (airlineFromDb != null)
        {
            if (airlineFromDb.Name != "" || flightData.AirlineName == null) return airlineFromDb;
            
            airlineFromDb.Name = flightData.AirlineName;
            DbContext.Airlines.Update(airlineFromDb);
            await DbContext.SaveChangesAsync();
            
            return airlineFromDb;
        }
        
        airlineFromDb = new Airline
        {
            Iata = flightData.AirlineIata ?? "",
            Icao = flightData.AirlineIcao ?? "",
            Name = flightData.AirlineName ?? ""
        };
        airlineFromDb = DbContext.Airlines.Add(airlineFromDb).Entity;
        await DbContext.SaveChangesAsync();

        return airlineFromDb;
    }

    private async Task<FlightStatus> GetOrCreateFlightStatusFromFlightData(FlightData flightData)
    {
        flightData.FlightStatus ??= "Unknown";
        var flightStatusFromDb = await DbContext.FlightStatuses.FirstOrDefaultAsync(fs =>
            fs.Name.ToLower() == flightData.FlightStatus.ToLower());

        if (flightStatusFromDb == null)
        {
            flightStatusFromDb = await DbContext.FlightStatuses.FirstAsync(fs =>
                fs.Name == "Unknown");
        }

        return flightStatusFromDb;
    }

    
    private async Task<Domain.Aircraft> GetOrCreateAircraftFromFlightData(FlightData flightData)
    {
        if (flightData.AircraftIcao == null)
        {
            throw new ArgumentException("Flight data is missing aircraft ICAO!");
        }
        var aircraftFromDb = await DbContext.Aircrafts
            .Include(a => a.AircraftModel)
            .FirstOrDefaultAsync(a => a.IcaoHex == flightData.AircraftIcao);

        aircraftFromDb ??= new Domain.Aircraft()
        {
            IcaoHex = flightData.AircraftIcao,
            RegistrationNumber = flightData.AircraftRegistration ?? "",
            InfoLastUpdatedUtc = DateTime.MinValue
        };
        
        if (flightData.AircraftModelCode != null &&
            aircraftFromDb.AircraftModel?.ModelCode == "")
        {
            aircraftFromDb.AircraftModel.ModelCode = flightData.AircraftModelCode.ToUpper();
            DbContext.AircraftModels.Update(aircraftFromDb.AircraftModel);
            await DbContext.SaveChangesAsync();
        }

        if (aircraftFromDb.AircraftModel != null)
        {
            return aircraftFromDb;
        }

        aircraftFromDb.AircraftModel = await GetOrCreateAircraftModel(flightData);
        aircraftFromDb = DbContext.Aircrafts.Add(aircraftFromDb).Entity;
        await DbContext.SaveChangesAsync();
            
        return aircraftFromDb;
    }

    private async Task<AircraftModel> GetOrCreateAircraftModel(FlightData flightData)
    {
        if (flightData.AircraftModelCode == null && flightData.AircraftModelName == null)
        {
            flightData.AircraftModelCode = "Unknown";
            flightData.AircraftModelName = "Unknown";
        }

        var aircraftModelFromDb = await DbContext.AircraftModels
            .FirstOrDefaultAsync(am =>
                am.ModelCode == flightData.AircraftModelCode ||
                am.ModelName == flightData.AircraftModelName);

        if (aircraftModelFromDb != null && aircraftModelFromDb.ModelCode != "Unknown" && 
            (aircraftModelFromDb.ModelCode == "" && flightData.AircraftModelCode != null || 
             aircraftModelFromDb.ModelName == "" && flightData.AircraftModelName != null))
        {
            if (aircraftModelFromDb.ModelCode == "" && flightData.AircraftModelCode != null)
            {
                aircraftModelFromDb.ModelCode = flightData.AircraftModelCode.ToUpper();
                aircraftModelFromDb = DbContext.AircraftModels.Update(aircraftModelFromDb).Entity;

            }
            if (aircraftModelFromDb.ModelName == "" && flightData.AircraftModelName != null)
            {
                aircraftModelFromDb.ModelName = flightData.AircraftModelName;
                aircraftModelFromDb = DbContext.AircraftModels.Update(aircraftModelFromDb).Entity;
            }
            await DbContext.SaveChangesAsync();
            return aircraftModelFromDb;
        }

        if (aircraftModelFromDb == null)
        {
            aircraftModelFromDb = new AircraftModel()
            {
                ModelCode = flightData.AircraftModelCode ?? "",
                ModelName = flightData.AircraftModelName ?? ""
            };
            aircraftModelFromDb = DbContext.AircraftModels.Add(aircraftModelFromDb).Entity;
            await DbContext.SaveChangesAsync();
        }
        return aircraftModelFromDb;
    }

}