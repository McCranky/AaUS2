using System;
using System.Collections.Generic;
using GeoLocApi.Data.Components;

namespace GeoLocApi.Models
{
    public class PlotModel
    {
        public Guid Id { get; set; }
        public int Number { get; set; }
        public string Description { get; set; }
        public List<string> Properties { get; set; }
        public GPS Gps { get; set; }
    }
}