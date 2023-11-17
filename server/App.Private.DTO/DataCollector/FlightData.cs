namespace App.Private.DTO.DataCollector;

public class FlightData
{
    public string? FlightIata { get; set; }
    public DateTime? ScheduledDepartureUtc { get; set; }
    public DateTime? ScheduledArrivalUtc { get; set; }
    public DateTime? EstimatedDepartureUtc { get; set; }
    public DateTime? EstimatedArrivalUtc { get; set; }
    public string? DepartureAirportIata { get; set; }
    public string? ArrivalAirportIata { get; set; }
    public string? DepartureTerminal { get; set; }
    public string? ArrivalTerminal { get; set; }
    public string? DepartureGate { get; set; }
    public string? ArrivalGate { get; set; }
    public string? FlightStatus { get; set; }
    public string? AirlineIata { get; set; }
    public string? AirlineIcao { get; set; }
    public string? AirlineName { get; set; }
    public string? AircraftIcao { get; set; }
    public string? AircraftRegistration { get; set; }
    public string? AircraftModelName { get; set; }
    public string? AircraftModelCode { get; set; }
}