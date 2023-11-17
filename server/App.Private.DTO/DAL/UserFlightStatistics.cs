using App.Private.DTO.IDTOs;

namespace App.Private.DTO.DAL;

public class UserFlightStatistics: IAirportsCoordinates
{
    public double DepartureAirportLatitude { get; set; }
    public double DepartureAirportLongitude { get; set; }
    
    public double ArrivalAirportLatitude { get; set; }
    public double ArrivalAirportLongitude { get; set; }

    public DateTime ScheduledDepartureUtc { get; set; }
    public DateTime ScheduledArrivalUtc { get; set; }
    
    public DateTime ExpectedDepartureUtc { get; set; }
    public DateTime ExpectedArrivalUtc { get; set; }
}