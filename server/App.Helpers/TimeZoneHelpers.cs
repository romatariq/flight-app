using App.Domain;
using Base.Helpers;

namespace App.Helpers;

public static class TimeZoneHelpers
{
    public static DateTime ConvertDateTimeToUtc(this DateTime fromDateTime, Airport fromAirport)
    {
        return fromDateTime.ConvertDateTimeToUtc((fromAirport.Latitude, fromAirport.Longitude));
    }
    
    public static DateTime ConvertDateTimeFromUtc(this DateTime fromDateTime, Airport toAirport)
    {
        return fromDateTime.ConvertDateTimeFromUtc((toAirport.Latitude, toAirport.Longitude));
    }

    public static DateTime ConvertDateTime(this DateTime fromDateTime, Airport fromAirport, Airport toAirport)
    {
        return fromDateTime.ConvertDateTime((fromAirport.Latitude, fromAirport.Longitude), (toAirport.Latitude, toAirport.Longitude));
    }
}