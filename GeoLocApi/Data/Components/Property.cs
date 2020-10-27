using System.Collections.Generic;

namespace GeoLocApi.Data.Components
{
    public class Property
    {
        public int RegisterNumber { get; set; }
        public string Description { get; set; }
        private readonly List<Plot> _plots;
        public GPS Gps { get; set; }

        public Property(int registerNumber, string description, GPS gps, List<Plot> plots = null)
        {
            RegisterNumber = registerNumber;
            Description = description;
            Gps = gps;
            _plots = plots ?? new List<Plot>();
        }

        public void AddPlot(Plot plot)
        {
            _plots.Add(plot);
        }

        public bool RemoveProperty(Plot plot)
        {
            return _plots.Remove(plot);
        }
    }
}