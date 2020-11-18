using GeoLocApi.Data.Components;

namespace GeoLocApi.Models.Requests
{
    /// <summary>
    /// Stores incoming body of new Plot which is required to be created
    /// </summary>
    public class CreatePlotRequest
    {
        public int Number { get; set; }
        public string Description { get; set; }
        public GPS Gps { get; set; }
    }
}