using App.Domain;

namespace App.Helpers;

public static class DistanceHelpers
{
    public static double GetDistanceInMeters(this Airport airport, Airport otherAirport)
    {
        return  Base.Helpers.DistanceHelpers.
            GetDistanceInMeters(airport.Latitude, airport.Longitude, otherAirport.Latitude, otherAirport.Longitude);
    }
    
    public static double GetDistanceInMeters(this Airport airport, Aircraft aircraft)
    {
        if (aircraft.Latitude == null || aircraft.Longitude == null)
        {
            throw new Exception("Aircraft has no location data");
        }
        return  Base.Helpers.DistanceHelpers.
            GetDistanceInMeters(airport.Latitude, airport.Longitude, aircraft.Latitude.Value, aircraft.Longitude.Value);
    }
}