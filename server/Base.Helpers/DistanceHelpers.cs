namespace Base.Helpers;

public static class DistanceHelpers
{
    
    // https://github.com/ghuntley/geocoordinate/blob/master/src/GeoCoordinatePortable/GeoCoordinate.cs
    public static double GetDistanceInMeters(double fromLatitude, double fromLongitude, double toLatitude, double toLongitude)
    {
        var d1 = fromLatitude * (Math.PI / 180.0);
        var num1 = fromLongitude * (Math.PI / 180.0);
        var d2 = toLatitude * (Math.PI / 180.0);
        var num2 = toLongitude * (Math.PI / 180.0) - num1;
        var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) + Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);
    
        return 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
    }
}