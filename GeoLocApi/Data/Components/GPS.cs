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
        public char LatitudeSymbol { get; set; }
        public char LongtitudeSymbol { get; set; } 
        public double Latitude { get; set; }
        public double Longtitude { get; set; }
        
        public GPS(){}
        public GPS(char latitudeSymbol, double latitude, char longtitudeSymbol, double longtitude)
        {
            Latitude = latitude;
            Longtitude = longtitude;
            LatitudeSymbol = latitudeSymbol;
            LongtitudeSymbol = longtitudeSymbol;
        }
    }
}