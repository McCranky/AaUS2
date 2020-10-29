using System;
using GeoLocApi.Data.Components;

namespace GeoLocApi.Models.Requests
{
    public class UpdatePlotRequest
    {
        public Guid Id { get; set; }
        public int Number { get; set; }
        public string Description { get; set; }
        public GPS Gps { get; set; }
    }
}