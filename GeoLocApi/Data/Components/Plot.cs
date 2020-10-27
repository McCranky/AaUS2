using System.Collections.Generic;

namespace GeoLocApi.Data.Components
{
    public class Plot
    {
        public int Number { get; set; }
        public string Description { get; set; }
        private readonly List<Property> _properties;
        public bool HasProperties => _properties != null;
        public GPS Gps { get; set; }

        public Plot(int number, string description, GPS gps, List<Property> properties = null)
        {
            Number = number;
            Description = description;
            Gps = gps;
            _properties = properties ?? new List<Property>();
        }

        public void AddProperty(Property prop)
        {
            _properties.Add(prop);
        }

        public bool RemoveProperty(Property prop)
        {
            return _properties.Remove(prop);
        }
    }
}