using System;
using GeoLocApi.Data.Components;

namespace GeoLocApi.Models.Requests
{
    public class UpdatePropertyRequest
    {
        public Guid Id { get; set; }
        public int RegisterNumber { get; set; }
        public string Description { get; set; }
        public GPS Gps { get; set; }
    }
}