namespace App.Private.DTO.DAL;

public class FlightInfoDetails: FlightInfo
{
    public DateTime EstimatedDepartureUtc { get; set; }
    public DateTime EstimatedArrivalUtc { get; set; }
    
    public Aircraft? Aircraft { get; set; }
    
    public string? DepartureTerminal { get; set; }
    public string? ArrivalTerminal { get; set; }
    
    public string? DepartureGate { get; set; }
    public string? ArrivalGate { get; set; }

    public Guid? UserFlightId { get; set; }

    public bool DisplayDepartureTerminal { get; set; }
    public bool DisplayArrivalTerminal { get; set; }
    
    public bool DisplayDepartureGate { get; set; }
    public bool DisplayArrivalGate { get; set; }
}