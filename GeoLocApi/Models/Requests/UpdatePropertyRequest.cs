using System;
using GeoLocApi.Data.Components;

namespace GeoLocApi.Models.Requests
{
    public class ModifyPropertyInfo
    {
        public int RegisterNumber { get; set; }
        public string Description { get; set; }
        public GPS Gps { get; set; }
    }
    /// <summary>
    /// Stores information from incoming body request form modifying Property
    /// </summary>
    public class UpdatePropertyRequest
    {
        public Guid Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        /// <summary>
        /// Modified information
        /// </summary>
        public ModifyPropertyInfo Property { get; set; }
    }
}