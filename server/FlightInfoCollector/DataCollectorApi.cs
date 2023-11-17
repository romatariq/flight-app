using System.Globalization;
using System.Text.Json.Nodes;
using App.Domain;
using App.Private.DTO.DataCollector;
using Base.Helpers;

namespace FlightInfoCollector;

public class DataCollectorApi: IDataCollector
{

    private HttpClient Client { get; set; }
    private string ApiKey { get; set; } = Environment.GetEnvironmentVariable("DOTENV_AIRLABS_API_KEY") ?? "YOUR-API-KEY";
    
    public DataCollectorApi()
    {
        Client = new HttpClient();
    }

    public async Task<List<FlightData>> GetAirportDepartures(string airportIata)
    {
        return await GetAirportFlightsAirlabs(airportIata, true);
    }

    public async Task<List<FlightData>> GetAirportArrivals(string airportIata)
    {
        return await GetAirportFlightsAirlabs(airportIata, false);
    }

    public async Task<FlightData?> GetFlightTechnicalInfo(Flight flight)
    {
        return await GetFlightDetailsAirlabs(flight.FlightIata);
    }

    public async Task<AircraftLiveData> GetAircraftLiveInfo(string icaoHex)
    {
        // icao has to be lower case
        // OpenSky Network has max ~400 requests per day
        var url = $"https://opensky-network.org/api/states/all?icao24={icaoHex.ToLower()}";
        var response = await Client.GetStringAsync(url);
        var states = JsonNode.Parse(response)?["states"];

        var aircraft = new AircraftLiveData();
        if (states == null) return aircraft;
        
        var latestState = states.AsArray()[0];
            
        var longitude = latestState?[5]?.ToString();
        var latitude = latestState?[6]?.ToString();
        var speedInMetersPerSecond = latestState?[9]?.ToString();

        if (double.TryParse(longitude, out var longitudeOut) && double.TryParse(latitude, out var latitudeOut))
        {
            aircraft.Longitude = longitudeOut;
            aircraft.Latitude = latitudeOut;
        }
        
        if (decimal.TryParse(speedInMetersPerSecond, out var speedInMetersPerSecondOut))
        {
            aircraft.Speed = speedInMetersPerSecondOut * (decimal)3.6;
        }
        return aircraft;
    }
    
    public void Dispose()
    {
        Client.Dispose();
    }

    // Airlabs has max 1000 request per month
    private async Task<FlightData?> GetFlightDetailsAirlabs(string flightIata)
    {
        if (ApiKey == "YOUR-API-KEY") return null;
        var url = $"https://airlabs.co/api/v9/flight?flight_iata={flightIata}&api_key={ApiKey}";
        var response = await Client.GetStringAsync(url);
        var aircraft = JsonNode.Parse(response)?["response"];
        
        return new FlightData()
        {
            AirlineName = aircraft?["airline_name"]?.ToString().NullIfEmpty(),
            AircraftIcao = aircraft?["hex"]?.ToString().ToUpper().NullIfEmpty(),
            AircraftRegistration = aircraft?["reg_number"]?.ToString().ToUpper().NullIfEmpty(),
            AircraftModelName = aircraft?["model"]?.ToString().NullIfEmpty(),
        };
    }
    

    private async Task<List<FlightData>> GetAirportFlightsAirlabs(string airportIata, bool isDeparture)
    {
        var direction = isDeparture ? "dep" : "arr";
        var url = $"https://airlabs.co/api/v9/schedules?{direction}_iata={airportIata}&api_key={ApiKey}";
        var response = await Client.GetStringAsync(url);
        var flightsList = new List<FlightData>();
        var flights = JsonNode.Parse(response)?["response"];
        
        if (flights == null) return flightsList;
        
        foreach (var flightJson in flights.AsArray())
        {
            var flight = GetFlightFromAirlabsJson(flightJson);
            if (!flightsList.Exists(f => 
                    f.ArrivalAirportIata == flight.ArrivalAirportIata && 
                    f.DepartureAirportIata == flight.DepartureAirportIata && 
                    f.ScheduledDepartureUtc == flight.ScheduledDepartureUtc &&
                    f.ScheduledArrivalUtc == flight.ScheduledArrivalUtc))
            {
                flightsList.Add(flight);
            }
        }

        return flightsList;
    }
    
    private string GetCorrectFlightStatus(JsonNode? flight)
    {
        var status = flight?["status"]?.ToString().NullIfEmpty();
        return status?.ToLower() switch
        {
            "scheduled" => "Scheduled",
            "active" => "Live",
            "landed" => "Finished",
            "cancelled" => "Cancelled",
            _ => "Unknown"
        };
    }

    
    private FlightData GetFlightFromAirlabsJson(JsonNode? flight)
    {
        var scheduledDeparture = flight?["dep_time_utc"]?.ToString().NullIfEmpty();
        var scheduledArrival = flight?["arr_time_utc"]?.ToString().NullIfEmpty();
        var estimatedDeparture = flight?["dep_estimated_utc"]?.ToString().NullIfEmpty();
        var estimatedArrival = flight?["arr_estimated_utc"]?.ToString().NullIfEmpty();

        var flightObject = new FlightData()
        {
            FlightIata = flight?["flight_iata"]?.ToString().ToUpper().NullIfEmpty(),
            DepartureAirportIata = flight?["dep_iata"]?.ToString().ToUpper().NullIfEmpty(),
            ArrivalAirportIata = flight?["arr_iata"]?.ToString().ToUpper().NullIfEmpty(),
            DepartureTerminal = flight?["dep_terminal"]?.ToString().ToUpper().NullIfEmpty(),
            ArrivalTerminal = flight?["arr_terminal"]?.ToString().ToUpper().NullIfEmpty(),
            DepartureGate = flight?["dep_gate"]?.ToString().ToUpper().NullIfEmpty(),
            ArrivalGate = flight?["arr_gate"]?.ToString().ToUpper().NullIfEmpty(),
            FlightStatus = GetCorrectFlightStatus(flight),
            AirlineIata = flight?["airline_iata"]?.ToString().ToUpper().NullIfEmpty(),
            AirlineIcao = flight?["airline_icao"]?.ToString().ToUpper().NullIfEmpty(),
        };

        if (scheduledDeparture == null || scheduledArrival == null) return flightObject;
        
        flightObject.ScheduledDepartureUtc = DateTime.ParseExact(scheduledDeparture, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
        flightObject.ScheduledArrivalUtc = DateTime.ParseExact(scheduledArrival, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
        
        flightObject.EstimatedDepartureUtc = estimatedDeparture == null ? 
            null :
            DateTime.ParseExact(estimatedDeparture, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
        flightObject.EstimatedArrivalUtc = estimatedArrival == null ? 
            null :
            DateTime.ParseExact(estimatedArrival, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
        return flightObject;
    }
}