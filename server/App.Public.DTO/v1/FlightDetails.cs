using System.ComponentModel.DataAnnotations;

namespace App.Public.DTO.v1;

/// <summary>
/// Detailed flight information.
/// </summary>
public class FlightDetails: Flight
{
    /// <summary>
    /// Estimated departure date and time in departure airport local time.
    /// </summary>
    public DateTime EstimatedDepartureLocal { get; set; }
    
    /// <summary>
    /// Estimated arrival date and time in arrival airport local time.
    /// </summary>
    public DateTime EstimatedArrivalLocal { get; set; }
    
    /// <summary>
    /// Estimated departure date and time in UTC.
    /// </summary>
    public DateTime EstimatedDepartureUtc { get; set; }
    
    /// <summary>
    /// Estimated arrival date and time in UTC.
    /// </summary>
    public DateTime EstimatedArrivalUtc { get; set; }
    
    /// <summary>
    /// Flight departure (from) terminal.
    /// </summary>
    public string? DepartureTerminal { get; set; }
    
    /// <summary>
    /// Flight arrival (to) terminal.
    /// </summary>
    public string? ArrivalTerminal { get; set; }
    
    /// <summary>
    /// Flight departure (from) gate.
    /// </summary>
    public string? DepartureGate { get; set; }
    
    /// <summary>
    /// Flight arrival (to) gate.
    /// </summary>
    public string? ArrivalGate { get; set; }

    /// <summary>
    /// Information about flight aircraft.
    /// </summary>
    public Aircraft? Aircraft { get; set; }

    /// <summary>
    /// Should departure terminal be displayed.
    /// </summary>
    public bool DisplayDepartureTerminal { get; set; }
    
    /// <summary>
    /// Should arrival terminal be displayed.
    /// </summary>
    public bool DisplayArrivalTerminal { get; set; }
    
    /// <summary>
    /// Should departure gate be displayed.
    /// </summary>
    public bool DisplayDepartureGate { get; set; }
    
    /// <summary>
    /// Should arrival gate be displayed.
    /// </summary>
    public bool DisplayArrivalGate { get; set; }

    /// <summary>
    /// Percentage of flight done (0-100).
    /// </summary>
    [Range(0, 100)]
    public int PercentageOfFlightDone { get; set; }
    
    /// <summary>
    /// Identification of user flight to see if user is tracking the flight.
    /// </summary>
    public Guid? UserFlightId { get; set; }
    
    /// <summary>
    /// Total flight distance in kilometers.
    /// </summary>
    public double FlightDistanceKm { get; set; }

    /// <summary>
    /// Average amount of kgs of CO2 created per person to travel the distance by car.
    /// </summary>
    public double CarKgOfCo2PerPerson { get; set; }
    
    /// <summary>
    /// Average amount of kgs of CO2 created per person to travel the distance by plane.
    /// </summary>
    public double PlaneKgOfCo2PerPerson { get; set; }
    
    /// <summary>
    /// Average amount of kgs of CO2 created per person to travel the distance by train.
    /// </summary>
    public double TrainKgOfCo2PerPerson { get; set; }
    
    /// <summary>
    /// Average amount of kgs of CO2 created per person to travel the distance by ship.
    /// </summary>
    public double ShipKgOfCo2PerPerson { get; set; }

    /// <summary>
    /// Average amount of minutes to travel the distance by car.
    /// </summary>
    public double CarTravelTimeMinutes { get; set; }
    
    /// <summary>
    /// Average amount of minutes to travel the distance by plane.
    /// </summary>
    public double PlaneTravelTimeMinutes { get; set; }
    
    /// <summary>
    /// Average amount of minutes to travel the distance by train.
    /// </summary>
    public double TrainTravelTimeMinutes { get; set; }
    
    /// <summary>
    /// Average amount of minutes to travel the distance by ship.
    /// </summary>
    public double ShipTravelTimeMinutes { get; set; }
}