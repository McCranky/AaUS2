using GeoLocApi.Data.Components;

namespace GeoLocApi.Models.Requests
{
    public class CreatePropertyRequest
    {
        public int RegisterNumber { get; set; }
        public string Description { get; set; }
        public GPS Gps { get; set; }
    }
}