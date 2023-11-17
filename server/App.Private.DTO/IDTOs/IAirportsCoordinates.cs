namespace App.Private.DTO.IDTOs;

public interface IAirportsCoordinates
{
    public double DepartureAirportLatitude { get; set; }
    public double DepartureAirportLongitude { get; set; }    
    
    public double ArrivalAirportLatitude { get; set; }
    public double ArrivalAirportLongitude { get; set; }
}