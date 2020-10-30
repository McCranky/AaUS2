using System;
using GeoLocApi.Data.Components;

namespace GeoLocApi.Models.Requests
{
    public class ModifyPropertyInfo
    {
        public Guid Id { get; set; }
        public int RegisterNumber { get; set; }
        public string Description { get; set; }
        public GPS Gps { get; set; }
    }
    public class UpdatePropertyRequest
    {
        public Guid Id { get; set; }
        public double Latitude { get; set; }
        public double Longtitude { get; set; }
        public ModifyPropertyInfo Property { get; set; }
    }
}