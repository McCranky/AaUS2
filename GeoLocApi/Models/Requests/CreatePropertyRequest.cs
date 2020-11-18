using GeoLocApi.Data.Components;

namespace GeoLocApi.Models.Requests
{
    /// <summary>
    /// Stores incoming body of new Property which is required to be created
    /// </summary>
    public class CreatePropertyRequest
    {
        public int RegisterNumber { get; set; }
        public string Description { get; set; }
        public GPS Gps { get; set; }
    }
}