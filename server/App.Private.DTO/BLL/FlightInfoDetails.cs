namespace App.Private.DTO.BLL;

public class FlightInfoDetails: FlightInfo
{
    public DateTime EstimatedDepartureLocal { get; set; }
    public DateTime EstimatedArrivalLocal { get; set; }
    
    public DateTime EstimatedDepartureUtc { get; set; }
    public DateTime EstimatedArrivalUtc { get; set; }
    
    public string? DepartureTerminal { get; set; }
    public string? ArrivalTerminal { get; set; }
    
    public string? DepartureGate { get; set; }
    public string? ArrivalGate { get; set; }

    public Aircraft? Aircraft { get; set; }

    public bool DisplayDepartureTerminal { get; set; }
    public bool DisplayArrivalTerminal { get; set; }
    
    public bool DisplayDepartureGate { get; set; }
    public bool DisplayArrivalGate { get; set; }

    public int PercentageOfFlightDone { get; set; }
    
    public Guid? UserFlightId { get; set; }

    public double FlightDistanceKm { get; set; }

    public double CarKgOfCo2PerPerson { get; set; }
    public double PlaneKgOfCo2PerPerson { get; set; }
    public double TrainKgOfCo2PerPerson { get; set; }
    public double ShipKgOfCo2PerPerson { get; set; }

    public double CarTravelTimeMinutes { get; set; }
    public double PlaneTravelTimeMinutes { get; set; }
    public double TrainTravelTimeMinutes { get; set; }
    public double ShipTravelTimeMinutes { get; set; }
}