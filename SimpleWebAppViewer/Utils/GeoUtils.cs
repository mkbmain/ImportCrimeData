namespace SimpleWebAppViewer.Utils;

public record LocationBounds(double MinLatitude, double MaxLatitude, double MinLongitude, double MaxLongitude);

public class GeoUtils
{
    private const double EarthRadiusInMiles = 3958.8;

    public static LocationBounds CalculateBounds(double latitude, double longitude, double radiusInMiles)
    {
        var radiansLat = DegreesToRadians(latitude);
        var radiansLon = DegreesToRadians(longitude);

        var angularDistance = radiusInMiles / EarthRadiusInMiles;
        var deltaLon = Math.Asin(Math.Sin(angularDistance) / Math.Cos(radiansLat));

        return new LocationBounds
        (
            MinLatitude: RadiansToDegrees(radiansLat - angularDistance),
            MaxLatitude: RadiansToDegrees(radiansLat + angularDistance),
            MinLongitude: RadiansToDegrees(radiansLon - deltaLon),
            MaxLongitude: RadiansToDegrees(radiansLon + deltaLon)
        );
    }

    private static double DegreesToRadians(double degrees) => degrees * (Math.PI / 180.0);
    private static double RadiansToDegrees(double radians) => radians * (180.0 / Math.PI);
}