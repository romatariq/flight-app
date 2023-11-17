using System.Text.Json.Nodes;
using App.Domain;
using App.Private.DTO.DataCollector;
using Base.Helpers;
using PuppeteerSharp;

namespace FlightInfoCollector;

public class DataCollectorWebScraper: IDataCollector
{

    private IBrowser? Browser { get; set; }

    private async Task InitBrowser()
    {
        if (Browser != null) return;
        var launchOptions = new LaunchOptions()
        {
            Headless = true,
            ExecutablePath = "C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe",
        };
        var connectionOptions = new ConnectOptions()
        {
            // BrowserWSEndpoint = Environment.GetEnvironmentVariable("DOTENV_BROWSER_ENDPOINT"),
            BrowserWSEndpoint = $"wss://chrome.browserless.io?token={Environment.GetEnvironmentVariable("DOTENV_BROWSERLESS_KEY")}",
        };

        Browser = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"
            ? await Puppeteer.LaunchAsync(launchOptions)
            : await Puppeteer.ConnectAsync(connectionOptions);
    }
    
    
    public async Task<List<FlightData>> GetAirportDepartures(string airportIata)
    {
        await InitBrowser();
        return await GetAirportFlightsFr24(airportIata, true);
    }
    
    public async Task<List<FlightData>> GetAirportArrivals(string airportIata)
    {
        await InitBrowser();
        return await GetAirportFlightsFr24(airportIata, false);
    }

    public Task<FlightData?> GetFlightTechnicalInfo(Flight flight)
    {
        return Task.FromResult<FlightData?>(null);
    }

    public async Task<AircraftLiveData> GetAircraftLiveInfo(string icaoHex)
    {
        await InitBrowser();
        return await GetAircraftLiveAdsb(icaoHex);
    }

    public void Dispose()
    {
        Browser?.CloseAsync();
        Browser?.Dispose();
        // GC.SuppressFinalize(this);
    }

    // TODO - fetch previous flights (click previous button at least once or until no flights that were more than an hour ago?) 
    private async Task<List<FlightData>> GetAirportFlightsFr24(string airportIata, bool isDeparture)
    {
        // https://api.flightradar24.com/common/v1/airport.json?code=tll&plugin[]=&plugin-setting[schedule][mode]=departures&plugin-setting[schedule][timestamp]=1679331720&page=-1&limit=100&fleet=&token=
        var direction = isDeparture ? "departures" : "arrivals";
        var url = $"https://www.flightradar24.com/data/airports/{airportIata.ToLower()}/{direction}";
        await using var page = await Browser!.NewPageAsync();
        var flightList = new List<FlightData>();
        var responseFound = false;

        page.Response += async (sender, e) =>
        {
            if (!e.Response.Url.Contains("api.flightradar24")) return;
            Console.WriteLine(e.Response.Url);
            
            var response = await e.Response.TextAsync();
            if (response == null) {
            
                responseFound = true;
                return;
            };
            
            var json = JsonNode.Parse(response);
            
            var flights = json?["result"]?["response"]?["airport"]?["pluginData"]?["schedule"]?[direction]?["data"];
            if (flights == null)
            {
                responseFound = true;
                return;
            }

            flightList.AddRange(
                flights.AsArray()
                    .Select(flight => GetFlightFromFr24Json(flight?["flight"], airportIata, isDeparture)));
            flightList.RemoveAll(flight => 
                flight.FlightIata == null || 
                flight.ScheduledDepartureUtc == null || 
                flight.ScheduledArrivalUtc == null);
            responseFound = true;
        };

        await page.GoToAsync(url);
        
        // wait for response
        var waitedForResponse = 0;
        while (!responseFound && waitedForResponse < 2000)
        {
            await Task.Delay(100);
            waitedForResponse += 100;
        }
        
        await page.CloseAsync();
        return flightList;
    }
    
    
    private FlightData GetFlightFromFr24Json(JsonNode? flight, string airportIata, bool isDeparture)
    {
        var scheduledDeparture10digit = flight?["time"]?["scheduled"]?["departure"]?.ToString().NullIfEmpty();
        var scheduledArrival10digit = flight?["time"]?["scheduled"]?["arrival"]?.ToString().NullIfEmpty();
        var estimatedDeparture10digit = flight?["time"]?["estimated"]?["departure"]?.ToString().NullIfEmpty();
        var estimatedArrival10digit = flight?["time"]?["estimated"]?["arrival"]?.ToString().NullIfEmpty();
        // dependent of direction
        var direction = isDeparture ? "destination" : "origin";
        var otherAirportIata = flight?["airport"]?[direction]?["code"]?["iata"]?.ToString().ToUpper().NullIfEmpty();
        airportIata = airportIata.ToUpper();

        var flightObject = new FlightData()
        {
            FlightIata = flight?["identification"]?["number"]?["default"]?.ToString().ToUpper().NullIfEmpty(),
            DepartureAirportIata = isDeparture ? airportIata : otherAirportIata,
            ArrivalAirportIata = isDeparture ? otherAirportIata : airportIata,
            DepartureTerminal = flight?["airport"]?["origin"]?["info"]?["terminal"]?.ToString().ToUpper().NullIfEmpty(),
            ArrivalTerminal = flight?["airport"]?["destination"]?["info"]?["terminal"]?.ToString().ToUpper().NullIfEmpty(),
            DepartureGate = flight?["airport"]?["origin"]?["info"]?["gate"]?.ToString().ToUpper().NullIfEmpty(),
            ArrivalGate = flight?["airport"]?["destination"]?["info"]?["gate"]?.ToString().ToUpper().NullIfEmpty(),
            FlightStatus = GetCorrectFlightStatus(flight),
            AirlineIata = flight?["airline"]?["code"]?["iata"]?.ToString().ToUpper().NullIfEmpty(),
            AirlineIcao = flight?["airline"]?["code"]?["icao"]?.ToString().ToUpper().NullIfEmpty(),
            AirlineName = flight?["airline"]?["short"]?.ToString().NullIfEmpty(),
            AircraftIcao = flight?["aircraft"]?["hex"]?.ToString().ToUpper().NullIfEmpty(),
            AircraftRegistration = flight?["aircraft"]?["registration"]?.ToString().ToUpper().NullIfEmpty(),
            AircraftModelName = flight?["aircraft"]?["model"]?["text"]?.ToString().NullIfEmpty(),
            AircraftModelCode = flight?["aircraft"]?["model"]?["code"]?.ToString().ToUpper().NullIfEmpty(),
        };

        if (scheduledDeparture10digit == null || scheduledArrival10digit == null) return flightObject;

        flightObject.ScheduledDepartureUtc = new DateTime(1970, 1, 1).AddSeconds(int.Parse(scheduledDeparture10digit));
        flightObject.ScheduledArrivalUtc = new DateTime(1970, 1, 1).AddSeconds(int.Parse(scheduledArrival10digit));
        flightObject.EstimatedDepartureUtc = estimatedDeparture10digit == null ? 
            null :
            new DateTime(1970, 1, 1).AddSeconds(int.Parse(estimatedDeparture10digit));
        flightObject.EstimatedArrivalUtc = estimatedArrival10digit == null ? 
            null :
            new DateTime(1970, 1, 1).AddSeconds(int.Parse(estimatedArrival10digit));
        return flightObject;
    }
    
    
    private string GetCorrectFlightStatus(JsonNode? flight)
    {
        var status = flight?["status"]?["generic"]?["status"]?["text"]?.ToString().NullIfEmpty();
        status = status?.ToLower() switch
        {
            "estimated" => "Estimated",
            "scheduled" => "Scheduled",
            "delayed" => "Scheduled",
            "departed" => "Live",
            "landed" => "Finished",
            "canceled" => "Cancelled",
            _ => "Unknown"
        };

        var isDeparture = flight?["status"]?["generic"]?["status"]?["type"]?.ToString().NullIfEmpty() == "departure";
        var live = flight?["status"]?["live"]?.ToString().NullIfEmpty() == "true";
        status = status == "Live" && !live ? "Finished" : status;
        status = status == "Estimated" && live && !isDeparture ? "Live" : status;
        status = status == "Estimated" ? "Scheduled" : status;
        return status;
    }

    
    private async Task<AircraftLiveData> GetAircraftLiveAdsb(string aircraftIcaoHex)
    {
        var url = $"https://globe.adsbexchange.com/?icao={aircraftIcaoHex.ToLower()}";
        
        await using var page = await Browser!.NewPageAsync();
        await page.GoToAsync(url, WaitUntilNavigation.Networkidle2);

        var aircraft = new AircraftLiveData();
        var positions = page.QuerySelectorAsync("#selected_position").Result
            .GetPropertyAsync("innerText").Result
            .JsonValueAsync().Result
            .ToString();
        var speed = page.QuerySelectorAsync("#selected_speed1").Result
            .GetPropertyAsync("innerText").Result
            .JsonValueAsync().Result
            .ToString();
        
        if (positions != null && !positions.Contains("n/a"))
        {
            var positionsArray = positions.Replace("°", "").Split(", ");
            if (double.TryParse(positionsArray[0], out var latitude) && double.TryParse(positionsArray[1], out var longitude))
            {
                aircraft.Latitude = latitude;
                aircraft.Longitude = longitude;
            }
        }
        if (speed != null && !speed.Contains("n/a") && decimal.TryParse(speed[..^3], out var speedDecimal))
        {
            aircraft.Speed = speedDecimal;
        }
        
        await page.CloseAsync();
        return aircraft;
    }


    // TODO - not finished
    private async Task<string> GetAirportDeparturesAvionio(string airportCode)
    {
        var url = $"https://www.avionio.com/en/airport/{airportCode}/departures";
        
        await using var page = await Browser!.NewPageAsync();
        await page.GoToAsync(url);

        var rows = await page.QuerySelectorAsync(".timetable").Result
            .QuerySelectorAsync("tbody").Result
            .QuerySelectorAllAsync("tr");
        
        foreach (var row in rows)
        {
            var classNames = row.GetPropertyAsync("className").Result.JsonValueAsync().Result.ToString();
            if (classNames!.Contains("timetable-childFlight")) continue;

            var columns = await row.QuerySelectorAllAsync("td");
            var scheduledTime = columns[0].GetPropertyAsync("innerText").Result.JsonValueAsync().Result.ToString();
            var scheduledDate = columns[1].GetPropertyAsync("innerText").Result.JsonValueAsync().Result.ToString();
            
            Console.WriteLine(classNames);
        }

        await page.CloseAsync();
        return "";
    }
}