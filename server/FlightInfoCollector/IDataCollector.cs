using App.Domain;
using App.Private.DTO.DataCollector;

namespace FlightInfoCollector;

public interface IDataCollector: IDisposable
{
    
    public Task<List<FlightData>> GetAirportDepartures(string airportIata);
    
    public Task<List<FlightData>> GetAirportArrivals(string airportIata);
    
    public Task<FlightData?> GetFlightTechnicalInfo(Flight flight);
    
    public Task<AircraftLiveData> GetAircraftLiveInfo(string aircraftIcaoHex);
}