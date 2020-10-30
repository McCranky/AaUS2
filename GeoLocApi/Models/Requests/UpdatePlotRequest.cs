using System;
using GeoLocApi.Data.Components;

namespace GeoLocApi.Models.Requests
{
    public class ModifyPlotInfo
    {
        public Guid Id { get; set; }
        public int Number { get; set; }
        public string Description { get; set; }
        public GPS Gps { get; set; }
    }
    public class UpdatePlotRequest
    {
        public Guid Id { get; set; }
        public double Latitude { get; set; }
        public double Longtitude { get; set; }
        public ModifyPlotInfo Plot { get; set; }
    }
}