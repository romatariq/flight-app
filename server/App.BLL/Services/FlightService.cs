using App.Contracts.BLL.IServices;
using App.Contracts.DAL;
using App.Contracts.DAL.IRepositories;
using App.Domain.Identity;
using App.Private.DTO.DataCollector;
using Base.BLL;
using Base.Contracts;
using Base.Helpers;
using FlightInfoCollector;
using Microsoft.EntityFrameworkCore;
using Bll = App.Private.DTO.BLL;
using Dal = App.Private.DTO.DAL;
using DomainDto = App.Domain;
using AutoMappers = App.Mappers.AutoMappers;

namespace App.BLL.Services;

public class FlightService :
    BaseEntityService<Dal.FlightInfo, Bll.FlightInfo, IFlightRepository>, IFlightService
{
    protected IAppUOW Uow;
    protected readonly IMapper<Dal.FlightInfoDetails, Bll.FlightInfoDetails> DetailsMapper;
    private readonly IDataCollector _dataCollector;


    public FlightService(IAppUOW uow, AutoMapper.IMapper mapper, IDataCollector dataCollector)
        : base(uow.FlightRepository, new AutoMappers.BLL.FlightMapper(mapper))
    {
        Uow = uow;
        DetailsMapper = new AutoMappers.BLL.FlightDetailsMapper(mapper);
        _dataCollector = dataCollector;
    }

    public async Task<IEnumerable<Bll.FlightInfo>> GetDepartures(string airportIata)
    {
        await FetchNewestDeparturesOrArrivalsIfNecessary(airportIata, true);
        return (await Uow.FlightRepository.GetDepartures(airportIata))
            .Select(Mapper.Map)!;
    }

    public async Task<IEnumerable<Bll.FlightInfo>> GetArrivals(string airportIata)
    {
        await FetchNewestDeparturesOrArrivalsIfNecessary(airportIata, false);
        return (await Uow.FlightRepository.GetArrivals(airportIata))
            .Select(Mapper.Map)!;
    }
    
    private async Task FetchNewestDeparturesOrArrivalsIfNecessary(string airportIata, bool isDeparture)
    {
        var airport = await Uow.AirportRepository.GetByIata(airportIata);
        if (airport == null || !airport.DisplayFlights)
        {
            throw new Exception("Airport error");
        }
        
        var flightsLastChecked = isDeparture ?
            airport.DeparturesLastCheckedUtc :
            airport.ArrivalsLastCheckedUtc;
        
        var dateNow = DateTime.UtcNow.RemoveKind();

        if (flightsLastChecked == null || flightsLastChecked.Value.AddMinutes(120) < dateNow)
        {
            List<FlightData>? flightDataList = null;
            if (isDeparture)
            {
                await Uow.AirportRepository.SetLastCheckedUtc(airport.Id, dateNow, true);
                await Uow.SaveChangesAsync();
                try
                {
                    flightDataList = await _dataCollector.GetAirportDepartures(airport.Iata);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    await Uow.AirportRepository.SetLastCheckedUtc(airport.Id, dateNow.AddMinutes(-120), true);
                    await Uow.SaveChangesAsync();
                }
            } else
            {
                await Uow.AirportRepository.SetLastCheckedUtc(airport.Id, dateNow, false);
                await Uow.SaveChangesAsync();
                try
                {
                    flightDataList = await _dataCollector.GetAirportArrivals(airport.Iata);
                } catch (Exception e)
                {
                    Console.WriteLine(e);
                    await Uow.AirportRepository.SetLastCheckedUtc(airport.Id, dateNow.AddMinutes(-120), false);
                    await Uow.SaveChangesAsync();
                }
            }

            if (flightDataList != null)
            {
                await Uow.FlightRepository.AddOrUpdateAsync(flightDataList);
                await Uow.SaveChangesAsync();
            }
        }
    }

    public async Task<Bll.FlightInfoDetails?> GetFlightAsync(Guid flightId, Guid? appUserId)
    {
        await Uow.FlightRepository.FetchNewestFlightAndUpdateIfNecessary(flightId, _dataCollector);
        await Uow.SaveChangesAsync();

        var flight = await Uow.FlightRepository.GetFlightAsync(flightId, appUserId);
        if (flight == null) return null;
        
        var mappedFlight = DetailsMapper.Map(flight);
        if (mappedFlight == null) return null;
        
        mappedFlight.ScheduledDepartureLocal = flight.ScheduledDepartureUtc.ConvertDateTimeFromUtc((flight.DepartureAirportLatitude, flight.DepartureAirportLongitude));
        mappedFlight.ScheduledArrivalLocal = flight.ScheduledArrivalUtc.ConvertDateTimeFromUtc((flight.ArrivalAirportLatitude, flight.ArrivalAirportLongitude));
        mappedFlight.EstimatedDepartureLocal = flight.EstimatedDepartureUtc.ConvertDateTimeFromUtc((flight.DepartureAirportLatitude, flight.DepartureAirportLongitude));
        mappedFlight.EstimatedArrivalLocal = flight.EstimatedArrivalUtc.ConvertDateTimeFromUtc((flight.ArrivalAirportLatitude, flight.ArrivalAirportLongitude));
        
        var flightTotalDistance = DistanceHelpers.GetDistanceInMeters(
            flight.DepartureAirportLatitude, flight.DepartureAirportLongitude, 
            flight.ArrivalAirportLatitude, flight.ArrivalAirportLongitude);

        mappedFlight = MapComparisonStats(mappedFlight, flightTotalDistance);

        if (flight.Status != "Live" ||
            flight.Aircraft?.Latitude == null || flight.Aircraft.Longitude == null)
        {
            if (flight.Status == "Finished")
            {
                mappedFlight.PercentageOfFlightDone = 100;
            }
            return mappedFlight;
        }

        var secondsFromLastChecked =
            (DateTime.UtcNow.RemoveKind() - flight.Aircraft.InfoLastUpdatedUtc).TotalSeconds;
        var speedMetersPerSecond =  (double) flight.Aircraft.SpeedKmh / 3.6;
        var metersFlownAfterLastChecked = secondsFromLastChecked * speedMetersPerSecond;
        var flightFlownDistance = DistanceHelpers.GetDistanceInMeters(
            flight.DepartureAirportLatitude, flight.DepartureAirportLongitude,
            flight.Aircraft.Latitude.Value, flight.Aircraft.Longitude.Value);
        mappedFlight.PercentageOfFlightDone = (int) ((flightFlownDistance + metersFlownAfterLastChecked) / flightTotalDistance * 100);
        return mappedFlight;
    }

    private Bll.FlightInfoDetails MapComparisonStats(Bll.FlightInfoDetails flight, double distanceMeters)
    {
        var co2Emissions = GetCarbonEmissions(distanceMeters / 1000);
        var averageTimes = GetAverageTimes(distanceMeters / 1000);
        flight.FlightDistanceKm = distanceMeters / 1000;
        flight.CarKgOfCo2PerPerson = co2Emissions.car;
        flight.PlaneKgOfCo2PerPerson = co2Emissions.plane;
        flight.TrainKgOfCo2PerPerson = co2Emissions.train;
        flight.ShipKgOfCo2PerPerson = co2Emissions.ship;
        
        flight.CarTravelTimeMinutes = averageTimes.car;
        flight.TrainTravelTimeMinutes = averageTimes.train;
        flight.ShipTravelTimeMinutes = averageTimes.ship;
        flight.PlaneTravelTimeMinutes = averageTimes.plane;
        return flight;
    }
    
    private (double car, double plane, double train, double ship) GetCarbonEmissions(double distanceKm)
    {
        // medium car
        const double carFuelConsumptionLitersPerKm = 0.1;
        const double petrolCo2KgPerLiter = 2.5;
        const double carPassengerCount = 5;
        
        // B737-800
        const double planeFuelConsumptionKgPerKm = 3.6;
        const double jetFuelCo2KgPerKg = 3.16;
        const double planePassengerCount = 190;
        
        // swedish electric train
        const double electricTrainEnergyConsumptionKwhPerKm = 4.2;
        const double electricityCo2KgPerKwh = 0.95;
        const double trainPassengerCount = 200;
        
        // cruise ship Norwegian Spirit
        const double shipFuelConsumptionLiterPerKm = 94;
        const double dieselCo2KgPerLiter = 2.68;
        const double shipPassengerCount = 2414;
        
        // returns as kg of CO2 per person
        return (
            car: distanceKm * carFuelConsumptionLitersPerKm * petrolCo2KgPerLiter / carPassengerCount,
            plane: distanceKm * planeFuelConsumptionKgPerKm * jetFuelCo2KgPerKg / planePassengerCount, 
            train: distanceKm * electricTrainEnergyConsumptionKwhPerKm * electricityCo2KgPerKwh / trainPassengerCount,
            ship: distanceKm * shipFuelConsumptionLiterPerKm * dieselCo2KgPerLiter / shipPassengerCount
        );
    }
    
    private (double car, double plane, double train, double ship) GetAverageTimes(double distanceKm)
    {
        // medium car
        const double carSpeedKmh = 100;
        
        // B737-800
        const double planeSpeedKmh = 840;
        
        // swedish electric train
        const double trainSpeedKmh = 160;
        
        // cruise ship Norwegian Spirit
        const double shipSpeedKmh = 45;
        
        // returns as minutes
        return (
                car: distanceKm / carSpeedKmh * 60,
                plane: distanceKm / planeSpeedKmh * 60,
                train: distanceKm / trainSpeedKmh * 60,
                ship: distanceKm / shipSpeedKmh * 60
        );
    }
}
