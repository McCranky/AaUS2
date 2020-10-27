namespace GeoLocApi.Data.Components
{
    public enum CardinalDirections
    {
        Nort = 'N',
        South = 'S',
        East = 'E',
        West = 'W'
    }
    
    public class GPS
    {
        public CardinalDirections LatitudeSymbol { get; set; }
        public CardinalDirections LongtitudeSymbol { get; set; } 
        public double Latitude { get; set; }
        public double Longtitude { get; set; }
        
        public GPS(CardinalDirections latitudeSymbol, double latitude, CardinalDirections longtitudeSymbol, double longtitude)
        {
            Latitude = latitude;
            Longtitude = longtitude;
            LatitudeSymbol = latitudeSymbol;
            LongtitudeSymbol = longtitudeSymbol;
        }
    }
}