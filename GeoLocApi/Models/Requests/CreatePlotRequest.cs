using GeoLocApi.Data.Components;

namespace GeoLocApi.Models.Requests
{
    public class CreatePlotRequest
    {
        public int Number { get; set; }
        public string Description { get; set; }
        public GPS Gps { get; set; }
    }
}