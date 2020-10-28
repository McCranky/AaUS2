using System;
using System.Collections.Generic;
using GeoLocApi.Data.Components;

namespace GeoLocApi.Models
{
    public class PropertyModel
    {
        public Guid Id { get; set; }
        public int RegisterNumber { get; set; }
        public string Description { get; set; }
        public List<string> Plots { get; set; }
        public GPS Gps { get; set; }
    }
}