namespace GeoLocApi.Data.Components
{
    public enum CardinalDirections
    {
        Nort = 'N',
        South = 'S',
        East = 'E',
        West = 'W'
    }
    /// <summary>
    /// Stores information about geographical location
    /// </summary>
    public class GPS
    {
        public char LatitudeSymbol { get; set; }
        public char LongitudeSymbol { get; set; } 
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        
        public GPS(){}
        public GPS(char latitudeSymbol, double latitude, char longitudeSymbol, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
            LatitudeSymbol = latitudeSymbol;
            LongitudeSymbol = longitudeSymbol;
        }
    }
}