using GeoTimeZone;

namespace Base.Helpers;

public static class TimeZoneHelpers
{
    public static DateTime RemoveKind(this DateTime dateTime)
    {
        return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
    }
    
    public static DateTime ConvertDateTimeToUtc(this DateTime fromDateTime, (double latitude, double longitude) fromAirportCoordinates)
    {
        var timeZoneName = TimeZoneLookup.GetTimeZone(fromAirportCoordinates.latitude, fromAirportCoordinates.longitude).Result;
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneName);
        
        var fromTimeDuplicate = new DateTime(fromDateTime.Year, fromDateTime.Month, fromDateTime.Day, fromDateTime.Hour, fromDateTime.Minute, fromDateTime.Second);
        return TimeZoneInfo.ConvertTimeToUtc(fromTimeDuplicate, timeZone).RemoveKind();
    }
    
    public static DateTime ConvertDateTimeFromUtc(this DateTime fromDateTime, (double latitude, double longitude) toAirportCoordinates)
    {
        var timeZoneName = TimeZoneLookup.GetTimeZone(toAirportCoordinates.latitude, toAirportCoordinates.longitude).Result;
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneName);
        
        var fromDateTimeDuplicate = new DateTime(fromDateTime.Year, fromDateTime.Month, fromDateTime.Day, fromDateTime.Hour, fromDateTime.Minute, fromDateTime.Second);
        return TimeZoneInfo.ConvertTimeFromUtc(fromDateTimeDuplicate, timeZone).RemoveKind();
    }

    public static DateTime ConvertDateTime(this DateTime fromDateTime, (double latitude, double longitude) fromAirportCoordinates, (double latitude, double longitude) toAirportCoordinates)
    {
        var fromTimeZoneName = TimeZoneLookup.GetTimeZone(fromAirportCoordinates.latitude, fromAirportCoordinates.longitude).Result;
        var toTimeZoneName = TimeZoneLookup.GetTimeZone(toAirportCoordinates.latitude, toAirportCoordinates.longitude).Result;

        var fromTimeZone = TimeZoneInfo.FindSystemTimeZoneById(fromTimeZoneName);
        var toTimeZone = TimeZoneInfo.FindSystemTimeZoneById(toTimeZoneName);
        
        // to use convertTime(DateTime, sourceTimeZone, destinationTimeZone) - DateTime cannot have Kind property (cannot make from Datetime.now())
        var fromDateTimeDuplicate = new DateTime(fromDateTime.Year, fromDateTime.Month, fromDateTime.Day, fromDateTime.Hour, fromDateTime.Minute, fromDateTime.Second);
        
        return TimeZoneInfo.ConvertTime(fromDateTimeDuplicate, fromTimeZone, toTimeZone).RemoveKind();
    }
}