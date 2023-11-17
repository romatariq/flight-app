using App.BLL.Services;
using App.DAL.EF;
using App.Domain;
using App.Private.DTO.DataCollector;
using AutoMapper;
using FlightInfoCollector;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit.Abstractions;

namespace Tests.Unit;

public class FlightServiceUnitTests
{
    
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly FlightService _service;
    private readonly AppDbContext _ctx;
    private readonly Mock<IDataCollector> _dataCollectorMock;
    
    private readonly Guid _tllGuid = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private readonly Guid _helGuid = Guid.Parse("00000000-0000-0000-0000-000000000002");
    private readonly Guid _airlineGuid = Guid.Parse("00000000-0000-0000-0000-000000000003");
    private readonly Guid _statusGuid = Guid.Parse("00000000-0000-0000-0000-000000000004");
    

    public FlightServiceUnitTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new App.Mappers.AutoMapperConfigs.BLLConfig());
        });
        
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
        _ctx = new AppDbContext(optionsBuilder.Options);
        var uow = new AppUOW(_ctx);

        // reset db
        _ctx.Database.EnsureDeleted();
        _ctx.Database.EnsureCreated();

        // create a fake verifiable dataCollector
        _dataCollectorMock = new Mock<IDataCollector>();
        _dataCollectorMock
            .Setup(x =>
                x.GetFlightTechnicalInfo(It.Is<Flight>(f => f.FlightIata == "FR123")))
            .ReturnsAsync(null as FlightData)
            .Verifiable();
        _dataCollectorMock
            .Setup(x =>
                x.GetFlightTechnicalInfo(It.Is<Flight>(f => f.FlightIata == "FR456")))
            .ReturnsAsync(null as FlightData)
            .Verifiable();
        
        _dataCollectorMock
            .Setup(x =>
                x.GetAircraftLiveInfo(It.Is<string>(f => f == "1234")))
            .ReturnsAsync(new AircraftLiveData()
            {
                Latitude = 59.865249635,
                Longitude = 24.898050305,
                Speed = 500
            })
            .Verifiable();
        
        _dataCollectorMock
            .Setup(x =>
                x.GetAirportDepartures(It.Is<string>(f => f == "TLL")))
            .ReturnsAsync(new List<FlightData>()
            {
                GetFlightExample()
            })
            .Verifiable();
        _dataCollectorMock
            .Setup(x =>
                x.GetAirportDepartures(It.Is<string>(f => f == "HEL")))
            .ReturnsAsync(new List<FlightData>())
            .Verifiable();
        
        _dataCollectorMock
            .Setup(x =>
                x.GetAirportArrivals(It.Is<string>(f => f == "HEL")))
            .ReturnsAsync(new List<FlightData>()
            {
                GetFlightExample()
            })
            .Verifiable();        
        _dataCollectorMock
            .Setup(x =>
                x.GetAirportArrivals(It.Is<string>(f => f == "TLL")))
            .ReturnsAsync(new List<FlightData>()) 
            .Verifiable();

        // SUT
        _service = new FlightService(uow, mapperConfig.CreateMapper(), _dataCollectorMock.Object);
        SeedDataAsync().Wait();
    }
    
    
    [Fact(DisplayName = "Airport not created yet")]
    public async Task TestNonExistingAirports()
    {
        // arrange
        const string airportIata = "RIX";
        
        // act and assert
        await Assert.ThrowsAnyAsync<Exception>(async () => await _service.GetDepartures(airportIata));
        _dataCollectorMock.Verify(
            x =>
                x.GetAirportDepartures(
                    It.Is<string>(iata => iata == airportIata)),
            Times.Never
        );
    }
    
    [Fact(DisplayName = "Airport created but not returning data")]
    public async Task TestExistingAirports()
    {
        // arrange
        const string airportIata = "BER";
        
        // act and assert
        await Assert.ThrowsAnyAsync<Exception>(async () => await _service.GetDepartures(airportIata));

        _dataCollectorMock.Verify(
            x =>
                x.GetAirportDepartures(
                    It.Is<string>(iata => iata == airportIata)),
            Times.Never
        );
    }
    
    [Fact(DisplayName = "Departures")]
    public async Task TestDepartures()
    {
        // arrange
        const string airportIata = "TLL";
        
        // act
        var res = (await _service.GetDepartures(airportIata)).ToList();
        var res2 = (await _service.GetDepartures(airportIata)).ToList();
        
        // assert
        Assert.NotEmpty(res);
        Assert.Equal(2, res.Count);
        
        Assert.NotEmpty(res2);
        Assert.Equal(2, res2.Count);

        Assert.NotNull(res.FirstOrDefault(f => f.FlightIata == "FR123"));
        Assert.NotNull(res2.FirstOrDefault(f => f.FlightIata == "FR456"));
        
        _dataCollectorMock.Verify(
            x =>
                x.GetAirportDepartures(
                    It.Is<string>(iata => iata == airportIata)),
            Times.Once
        );
    }
    
    [Fact(DisplayName = "No departures")]
    public async Task TestNoDepartures()
    {
        // arrange
        const string airportIata = "HEL";
        
        // act
        var res = (await _service.GetDepartures(airportIata)).ToList();
        var res2 = (await _service.GetDepartures(airportIata)).ToList();
        
        // assert
        Assert.Empty(res);
        Assert.Empty(res2);

        _dataCollectorMock.Verify(
            x =>
                x.GetAirportDepartures(
                    It.Is<string>(iata => iata == airportIata)),
            Times.Once
        );
    }
    
    [Fact(DisplayName = "Arrivals")]
    public async Task TestArrivals()
    {
        // arrange
        const string airportIata = "HEL";
        
        // act
        var res = (await _service.GetArrivals(airportIata)).ToList();
        var res2 = (await _service.GetArrivals(airportIata)).ToList();
        
        // assert
        Assert.NotEmpty(res);
        Assert.Equal(2, res.Count);
        
        Assert.NotEmpty(res2);
        Assert.Equal(2, res2.Count);
        
        Assert.NotNull(res.FirstOrDefault(f => f.FlightIata == "FR123"));
        Assert.NotNull(res2.FirstOrDefault(f => f.FlightIata == "FR456"));

        _dataCollectorMock.Verify(
            x =>
                x.GetAirportArrivals(
                    It.Is<string>(iata => iata == airportIata)),
            Times.Once
        );
    }
    
    [Fact(DisplayName = "No arrivals")]
    public async Task TestNoArrivals()
    {
        // arrange
        const string airportIata = "TLL";
        
        // act
        var res = (await _service.GetArrivals(airportIata)).ToList();
        var res2 = (await _service.GetArrivals(airportIata)).ToList();
        
        // assert
        Assert.Empty(res);
        Assert.Empty(res2);

        _dataCollectorMock.Verify(
            x =>
                x.GetAirportArrivals(
                    It.Is<string>(iata => iata == airportIata)),
            Times.Once
        );
    }
    
    [Fact(DisplayName = "Flight details")]
    public async Task TestFlight()
    {
        const string flightIata = "FR456";
        var localTime = DateTime.Now;
        var flight = await _ctx.Flights
            .Include(f => f.Aircraft)
            .FirstAsync(x => x.FlightIata == flightIata);

        var res = await _service.GetFlightAsync(flight.Id, null);
        
        Assert.NotNull(res);
        Assert.Equal(flightIata, res.FlightIata);
        Assert.Equal(50, res.PercentageOfFlightDone);
        Assert.Equal(101, Math.Round(res.FlightDistanceKm));
        
        Assert.Equal(0, (localTime.AddMinutes(-15) - res.ScheduledDepartureLocal).Minutes);
        Assert.Equal(0, (localTime.AddMinutes(15) - res.ScheduledArrivalLocal).Minutes);
        
        Assert.True(res.PlaneTravelTimeMinutes < res.TrainTravelTimeMinutes);
        Assert.True(res.TrainTravelTimeMinutes < res.CarTravelTimeMinutes);
        Assert.True(res.CarTravelTimeMinutes < res.ShipTravelTimeMinutes);

        Assert.True(res.ShipKgOfCo2PerPerson > res.PlaneKgOfCo2PerPerson);
        Assert.True(res.PlaneKgOfCo2PerPerson > res.CarKgOfCo2PerPerson);
        Assert.True(res.CarKgOfCo2PerPerson > res.TrainKgOfCo2PerPerson);
        
        Assert.Equal(11, Math.Round(res.ShipKgOfCo2PerPerson));
        Assert.Equal(6, Math.Round(res.PlaneKgOfCo2PerPerson));
        Assert.Equal(5, Math.Round(res.CarKgOfCo2PerPerson));
        Assert.Equal(2, Math.Round(res.TrainKgOfCo2PerPerson));
        
        _dataCollectorMock.Verify(
            x =>
                x.GetAircraftLiveInfo(
                    It.Is<string>(icao => icao == flight.Aircraft!.IcaoHex)),
            Times.Once
        );
        _dataCollectorMock.Verify(
            x =>
                x.GetFlightTechnicalInfo(
                    It.Is<Flight>(f => f.FlightIata == flightIata)),
            Times.Never
        );
    }
    
    [Fact(DisplayName = "Flight details")]
    public async Task TestNoFlight()
    {
        var flightId = Guid.NewGuid();
        
        var res = await _service.GetFlightAsync(flightId, null);
        
        Assert.Null(res);
    }
    
    [Fact(DisplayName = "Base service delete")]
    public async Task TestBaseService()
    {
        const string flightIata = "DELETE";
        var flight = new Flight()
        {
            FlightIata = flightIata,
            FlightStatusId = _statusGuid,
            DepartureAirportId = _tllGuid,
            ArrivalAirportId = _helGuid,
            AirlineId = _airlineGuid,
            ScheduledDepartureUtc = DateTime.UtcNow,
            ScheduledArrivalUtc = DateTime.UtcNow,
            ExpectedDepartureUtc = DateTime.UtcNow,
            ExpectedArrivalUtc = DateTime.UtcNow,
        };

        var flightsInitial = (await _service.GetDepartures("TLL"));
        Assert.Null(flightsInitial.FirstOrDefault(f => f.FlightIata == flightIata));

        await _ctx.AddAsync(flight);
        await _ctx.SaveChangesAsync();
        var flightsAfterAdd = await _service.GetDepartures("TLL");
        var flightToDelete = flightsAfterAdd.FirstOrDefault(f => f.FlightIata == flightIata);
        Assert.NotNull(flightToDelete);
        
        await _service.DeleteAsync(flightToDelete.Id);
        var flightsAfterDelete = await _service.GetDepartures("TLL");
        await _ctx.SaveChangesAsync();
        Assert.Null(flightsAfterDelete.FirstOrDefault(f => f.FlightIata == flightIata));
    }


    private async Task SeedDataAsync()
    {
        await _ctx.FlightStatuses.AddRangeAsync(new List<FlightStatus>()
        {
            new() { Name = "Scheduled" },
            new() { Name = "Live", Id = _statusGuid},
            new() { Name = "Finished" },
            new() { Name = "Cancelled" },
            new() { Name = "Unknown" },
        });
        await _ctx.Airports.AddRangeAsync(new List<Airport>()
        {
            new()
            {
                Id = _tllGuid,
                Iata = "TLL",
                Name = "Tallinn",
                DisplayAirport = true,
                Latitude = 59.41329956,
                Longitude = 24.83279991,
                Country = new Country()
                {
                    Iso2 = "EE",
                    Iso3 = "EST",
                    Name = "Estonia"
                }
            },
            new()
            {
                Id = _helGuid,
                Iata = "HEL",
                Name = "Helsinki",
                DisplayAirport = true,
                Latitude = 60.31719971,
                Longitude = 24.9633007,
                Country = new Country()
                {
                    Iso2 = "FI",
                    Iso3 = "FIN",
                    Name = "Finland"
                }
            },
            new()
            {
                Iata = "BER",
                Name = "Berlin",
                Latitude = 52.35138889,
                Longitude = 13.49388889,
                Country = new Country()
                {
                    Iso2 = "DE",
                    Iso3 = "DEU",
                    Name = "Germany"
                }
            },
        });
        await _ctx.Flights.AddAsync(new Flight()
        {
            DepartureAirportId = _tllGuid,
            ArrivalAirportId = _helGuid,
            FlightStatusId = _statusGuid,
            FlightIata = "FR456",
            ScheduledDepartureUtc = DateTime.UtcNow.AddMinutes(-15),
            ScheduledArrivalUtc = DateTime.UtcNow.AddMinutes(15),
            ExpectedDepartureUtc = DateTime.UtcNow.AddMinutes(-15),
            ExpectedArrivalUtc = DateTime.UtcNow.AddMinutes(15),
            Airline = new Airline()
            {
                Id = _airlineGuid,
                Name = "Ryanair",
                Iata = "FR",
                Icao = "RYR"
            },
            Aircraft = new Aircraft()
            {
                IcaoHex = "1234",
                RegistrationNumber = "foo-bar",
                AircraftModel = new AircraftModel()
                {
                    ModelCode = "B738",
                    ModelName = "Boeing 737-800"
                }
            }
        });
        await _ctx.SaveChangesAsync();
    }
    
    private static FlightData GetFlightExample()
    {
        return new FlightData
        {
            FlightIata = "FR123",
            ScheduledDepartureUtc = DateTime.UtcNow.AddHours(2),
            ScheduledArrivalUtc = DateTime.UtcNow.AddHours(2).AddMinutes(30),
            EstimatedDepartureUtc = DateTime.UtcNow.AddHours(2),
            EstimatedArrivalUtc = DateTime.UtcNow.AddHours(2).AddMinutes(30),
            DepartureAirportIata = "TLL",
            ArrivalAirportIata = "HEL",
            DepartureTerminal = null,
            ArrivalTerminal = null,
            DepartureGate = null,
            ArrivalGate = null,
            FlightStatus = "Scheduled",
            AirlineIata = "FR",
            AirlineIcao = "RYR",
            AirlineName = "Ryanair",
            AircraftIcao = "4601F6",
            AircraftRegistration = "OH-ATI",
            AircraftModelCode = "AT75",
            AircraftModelName = "ATR 72-500",
        };
    }
}